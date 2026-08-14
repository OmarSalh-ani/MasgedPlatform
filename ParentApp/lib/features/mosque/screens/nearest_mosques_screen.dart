import 'dart:async';

import 'package:flutter/material.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:geolocator/geolocator.dart';
import '../../../core/theme/app_colors.dart';
import '../../../core/services/mosque_service.dart';
import '../../../core/utils/maps_launcher.dart';
import 'package:flutter_map/flutter_map.dart';
import 'package:latlong2/latlong.dart';

/// Leaflet-style Carto basemaps (flutter_map = Flutter Leaflet).
const _leafletVoyagerUrl =
    'https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}.png';
const _leafletLightUrl =
    'https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}.png';
const _leafletDarkUrl =
    'https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}.png';

/// Cheap marker label style — avoid GoogleFonts per marker (main-thread cost).
const _markerLabelStyle = TextStyle(
  fontSize: 9,
  fontWeight: FontWeight.bold,
  color: AppColors.textPrimary,
  height: 1.1,
);

class NearestMosquesScreen extends StatefulWidget {
  const NearestMosquesScreen({super.key});

  @override
  State<NearestMosquesScreen> createState() => _NearestMosquesScreenState();
}

class _NearestMosquesScreenState extends State<NearestMosquesScreen> {
  static const int _initialMapLimit = 10;

  bool _isLoading = true;
  bool _isLoadingMore = false;
  bool _isMapView = false;
  List<Map<String, dynamic>> _mosques = [];
  String? _errorMessage;
  Position? _currentPosition;

  int _mapMarkerLimit = _initialMapLimit;
  double _searchRadiusMeters = 4000;
  List<Map<String, dynamic>> _mapMosquesVisible = [];

  String _currentTileUrl = _leafletVoyagerUrl;
  final List<Map<String, String>> _tileOptions = [
    {'name': 'Leaflet — Voyager', 'url': _leafletVoyagerUrl},
    {'name': 'Leaflet — فاتحة', 'url': _leafletLightUrl},
    {'name': 'Leaflet — داكنة', 'url': _leafletDarkUrl},
  ];

  @override
  void initState() {
    super.initState();
    _fetchMosques();
  }

  void _syncMapMosquesVisible() {
    final end = _mapMarkerLimit.clamp(0, _mosques.length);
    _mapMosquesVisible = end == 0 ? [] : _mosques.sublist(0, end);
  }

  Future<void> _fetchMosques({double? radius}) async {
    if (!mounted) return;
    setState(() {
      _isLoading = true;
      _errorMessage = null;
      if (radius == null) {
        _mapMarkerLimit = _initialMapLimit;
        _searchRadiusMeters = 4000;
        _mosques = [];
        _mapMosquesVisible = [];
      }
    });

    try {
      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
        if (permission == LocationPermission.denied) {
          if (!mounted) return;
          setState(() {
            _isLoading = false;
            _errorMessage = 'يجب تفعيل إذن الموقع لعرض المساجد القريبة';
          });
          return;
        }
      }

      if (permission == LocationPermission.deniedForever) {
        if (!mounted) return;
        setState(() {
          _isLoading = false;
          _errorMessage = 'إذن الموقع مرفوض تماماً، يرجى تفعيله من الإعدادات';
        });
        return;
      }

      final serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        if (!mounted) return;
        setState(() {
          _isLoading = false;
          _errorMessage = 'يرجى تفعيل خدمة الموقع (GPS) من إعدادات الجهاز';
        });
        return;
      }

      Position? position = _currentPosition;
      if (position == null) {
        try {
          position = await Geolocator.getCurrentPosition(
            locationSettings: const LocationSettings(
              accuracy: LocationAccuracy.medium,
              timeLimit: Duration(seconds: 15),
            ),
          );
        } catch (_) {
          position = await Geolocator.getLastKnownPosition();
        }
      }

      if (position == null) {
        if (!mounted) return;
        setState(() {
          _isLoading = false;
          _errorMessage = 'تعذر تحديد موقعك. حاول مرة أخرى في مكان مفتوح';
        });
        return;
      }

      final fetchRadius = radius ?? _searchRadiusMeters;
      final mosques = await MosqueService().getNearbyMosques(
        position,
        radius: fetchRadius,
      );

      if (!mounted) return;
      setState(() {
        if (radius == null) {
          _mosques = mosques;
        } else {
          _mosques = _mergeMosques(_mosques, mosques);
        }
        _currentPosition = position;
        _searchRadiusMeters = fetchRadius;
        _isLoading = false;
        _syncMapMosquesVisible();
      });
    } catch (e) {
      debugPrint('NearestMosques fetch error: $e');
      if (!mounted) return;
      setState(() {
        _isLoading = false;
        _errorMessage = 'حدث خطأ أثناء جلب البيانات. يرجى المحاولة لاحقاً';
      });
    }
  }

  List<Map<String, dynamic>> _mergeMosques(
    List<Map<String, dynamic>> existing,
    List<Map<String, dynamic>> incoming,
  ) {
    final byKey = <String, Map<String, dynamic>>{};
    for (final m in existing) {
      final lat = m['lat'] as double;
      final lon = m['lon'] as double;
      byKey['${lat.toStringAsFixed(5)}_${lon.toStringAsFixed(5)}'] = m;
    }
    for (final m in incoming) {
      final lat = m['lat'] as double;
      final lon = m['lon'] as double;
      final key = '${lat.toStringAsFixed(5)}_${lon.toStringAsFixed(5)}';
      byKey.putIfAbsent(key, () => m);
    }
    final merged = byKey.values.toList()
      ..sort((a, b) => (a['distance'] as double).compareTo(b['distance'] as double));
    return merged;
  }

  int _markerLimitForZoom(double zoom) {
    if (zoom >= 15) return 10;
    if (zoom >= 13) return 20;
    if (zoom >= 11) return 35;
    return 50;
  }

  double _radiusForZoom(double zoom) {
    if (zoom >= 15) return 4000;
    if (zoom >= 13) return 6000;
    if (zoom >= 11) return 9000;
    return 12000;
  }

  Future<void> _onZoomSettled(double zoom) async {
    final newLimit = _markerLimitForZoom(zoom);
    final newRadius = _radiusForZoom(zoom);
    final limitChanged = newLimit != _mapMarkerLimit;
    final needsMoreData =
        newRadius > _searchRadiusMeters && _mosques.length < newLimit;

    if (!limitChanged && !needsMoreData) return;

    if (limitChanged && mounted) {
      setState(() {
        _mapMarkerLimit = newLimit;
        _syncMapMosquesVisible();
      });
    }

    if (needsMoreData && !_isLoadingMore && _currentPosition != null) {
      if (mounted) setState(() => _isLoadingMore = true);
      try {
        final more = await MosqueService().getNearbyMosques(
          _currentPosition!,
          radius: newRadius,
        );
        if (!mounted) return;
        setState(() {
          _mosques = _mergeMosques(_mosques, more);
          _searchRadiusMeters = newRadius;
          _mapMarkerLimit = newLimit;
          _syncMapMosquesVisible();
        });
      } catch (e) {
        debugPrint('Load more mosques error: $e');
      } finally {
        if (mounted) setState(() => _isLoadingMore = false);
      }
    }
  }

  Future<void> _openInMaps(double lat, double lon) async {
    final opened = await MapsLauncher.openDirections(lat: lat, lon: lon);
    if (!opened && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(
            'تعذر فتح تطبيق الخرائط. ثبّت Google Maps أو جرّب مرة أخرى',
            style: AppFonts.cairo(),
          ),
        ),
      );
    }
  }

  String _formatDistance(double meters) {
    if (meters < 1000) {
      return '${meters.round()} متر';
    } else {
      return '${(meters / 1000).toStringAsFixed(1)} كم';
    }
  }

  String _estimateTime(double meters) {
    final minutes = (meters / 83).round();
    if (minutes < 1) return 'أقل من دقيقة';
    if (minutes > 60) {
      final hours = (minutes / 60).floor();
      final remainingMinutes = minutes % 60;
      return '$hours ساعة و $remainingMinutes دقيقة';
    }
    return '$minutes دقيقة مشي';
  }

  void _showTileSelector() {
    showModalBottomSheet(
      context: context,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(24))),
      builder: (context) {
        return Container(
          padding: const EdgeInsets.symmetric(vertical: 24),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'اختر نمط الخريطة',
                style: AppFonts.cairo(fontWeight: FontWeight.bold, fontSize: 18),
              ),
              const SizedBox(height: 16),
              ..._tileOptions.map((option) => ListTile(
                title: Text(option['name']!, style: AppFonts.cairo()),
                leading: Icon(
                  Icons.map_rounded,
                  color: _currentTileUrl == option['url'] ? AppColors.primary : Colors.grey,
                ),
                trailing: _currentTileUrl == option['url'] ? const Icon(Icons.check_circle, color: AppColors.primary) : null,
                onTap: () {
                  setState(() => _currentTileUrl = option['url']!);
                  Navigator.pop(context);
                },
              )),
            ],
          ),
        );
      },
    );
  }

  void _toggleMapView() {
    setState(() {
      _isMapView = !_isMapView;
      if (_isMapView) {
        _mapMarkerLimit = _initialMapLimit;
        _syncMapMosquesVisible();
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Directionality(
      textDirection: TextDirection.rtl,
      child: Scaffold(
        backgroundColor: AppColors.background,
        appBar: AppBar(
          backgroundColor: Colors.white,
          elevation: 0,
          title: Text(
            'أقرب مسجد',
            style: AppFonts.cairo(fontWeight: FontWeight.bold),
          ),
          leading: IconButton(
            icon: const Icon(Icons.arrow_back_ios_new_rounded, color: AppColors.textPrimary),
            onPressed: () => Navigator.pop(context),
          ),
          actions: [
            if (_isMapView)
              IconButton(
                icon: const Icon(Icons.layers_rounded, color: AppColors.primary),
                onPressed: _showTileSelector,
              ),
            IconButton(
              icon: Icon(_isMapView ? Icons.list_rounded : Icons.map_rounded, color: AppColors.primary),
              onPressed: _toggleMapView,
            ),
            IconButton(
              icon: const Icon(Icons.refresh_rounded, color: AppColors.primary),
              onPressed: () => _fetchMosques(),
            ),
          ],
        ),
        body: _isLoading
            ? const Center(child: CircularProgressIndicator())
            : _errorMessage != null
                ? Center(
                    child: Padding(
                      padding: const EdgeInsets.all(20),
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: [
                          Icon(Icons.location_off_rounded, size: 64, color: AppColors.textSecondary.withValues(alpha: 0.3)),
                          const SizedBox(height: 16),
                          Text(
                            _errorMessage!,
                            textAlign: TextAlign.center,
                            style: AppFonts.cairo(color: AppColors.textSecondary),
                          ),
                          const SizedBox(height: 16),
                          ElevatedButton(
                            onPressed: () => _fetchMosques(),
                            child: const Text('إعادة المحاولة'),
                          ),
                        ],
                      ),
                    ),
                  )
                : _isMapView
                    ? _NearestMosquesMapPanel(
                        key: ValueKey('map-$_currentTileUrl-${_mapMosquesVisible.length}'),
                        tileUrl: _currentTileUrl,
                        position: _currentPosition,
                        mapMosques: _mapMosquesVisible,
                        totalMosques: _mosques.length,
                        isLoadingMore: _isLoadingMore,
                        onZoomSettled: _onZoomSettled,
                        onOpenDirections: _openInMaps,
                      )
                    : _buildListView(),
      ),
    );
  }

  Widget _buildListView() {
    if (_mosques.isEmpty) {
      return Center(
        child: Text(
          'لم يتم العثور على مساجد قريبة',
          style: AppFonts.cairo(color: AppColors.textSecondary),
        ),
      );
    }
    return ListView.builder(
      padding: const EdgeInsets.all(16),
      itemCount: _mosques.length,
      cacheExtent: 400,
      itemBuilder: (context, index) {
        final mosque = _mosques[index];
        final distance = mosque['distance'] as double;
        final name = mosque['name'] as String;
        final address = mosque['address'] as String;
        final lat = mosque['lat'] as double;
        final lon = mosque['lon'] as double;

        return Container(
          margin: const EdgeInsets.only(bottom: 16),
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(24),
            border: Border.all(color: AppColors.border),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withValues(alpha: 0.03),
                blurRadius: 10,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(12),
                decoration: const BoxDecoration(
                  color: AppColors.primaryLight,
                  shape: BoxShape.circle,
                ),
                child: const Icon(Icons.mosque_rounded, color: AppColors.primary, size: 28),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      name,
                      style: AppFonts.cairo(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: AppColors.textPrimary,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Row(
                      children: [
                        const Icon(Icons.location_on_outlined, size: 14, color: AppColors.textSecondary),
                        const SizedBox(width: 4),
                        Expanded(
                          child: Text(
                            address,
                            style: AppFonts.cairo(
                              fontSize: 12,
                              color: AppColors.textSecondary,
                            ),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Text(
                          _formatDistance(distance),
                          style: AppFonts.cairo(fontSize: 13, color: AppColors.primary),
                        ),
                        const SizedBox(width: 12),
                        Text(
                          _estimateTime(distance),
                          style: AppFonts.cairo(fontSize: 13, color: AppColors.textSecondary),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              IconButton(
                icon: const Icon(Icons.directions_rounded, color: AppColors.primary, size: 28),
                onPressed: () => _openInMaps(lat, lon),
              ),
            ],
          ),
        );
      },
    );
  }
}

/// Isolated map widget — avoids rebuilding list + app bar on every pan frame.
class _NearestMosquesMapPanel extends StatefulWidget {
  const _NearestMosquesMapPanel({
    super.key,
    required this.tileUrl,
    required this.position,
    required this.mapMosques,
    required this.totalMosques,
    required this.isLoadingMore,
    required this.onZoomSettled,
    required this.onOpenDirections,
  });

  final String tileUrl;
  final Position? position;
  final List<Map<String, dynamic>> mapMosques;
  final int totalMosques;
  final bool isLoadingMore;
  final Future<void> Function(double zoom) onZoomSettled;
  final Future<void> Function(double lat, double lon) onOpenDirections;

  @override
  State<_NearestMosquesMapPanel> createState() => _NearestMosquesMapPanelState();
}

class _NearestMosquesMapPanelState extends State<_NearestMosquesMapPanel> {
  static const _cartoSubdomains = ['a', 'b', 'c', 'd'];

  final MapController _mapController = MapController();
  Timer? _zoomDebounce;
  bool _mapReady = false;
  bool _ignoreMapEvents = false;
  bool _pendingFit = false;
  List<Marker> _markers = [];
  double _lastZoomBucket = 14;

  @override
  void initState() {
    super.initState();
    _rebuildMarkers();
  }

  @override
  void didUpdateWidget(_NearestMosquesMapPanel oldWidget) {
    super.didUpdateWidget(oldWidget);
    final dataChanged = oldWidget.mapMosques != widget.mapMosques ||
        oldWidget.position != widget.position;
    if (dataChanged) {
      _rebuildMarkers();
      _pendingFit = true;
      setState(() {});
      if (_mapReady) {
        WidgetsBinding.instance.addPostFrameCallback((_) => _fitToMosques());
      }
    }
  }

  @override
  void dispose() {
    _zoomDebounce?.cancel();
    _mapReady = false;
    _mapController.dispose();
    super.dispose();
  }

  void _rebuildMarkers() {
    final markers = <Marker>[];
    final position = widget.position;

    if (position != null) {
      markers.add(
        Marker(
          point: LatLng(position.latitude, position.longitude),
          width: 36,
          height: 36,
          child: const Icon(Icons.person_pin_circle, color: Colors.blue, size: 32),
        ),
      );
    }

    for (final mosque in widget.mapMosques) {
      final name = mosque['name'] as String;
      final lat = mosque['lat'] as double;
      final lon = mosque['lon'] as double;

      markers.add(
        Marker(
          point: LatLng(lat, lon),
          width: 88,
          height: 40,
          alignment: Alignment.bottomCenter,
          child: _MosqueMapPin(
            name: name,
            onTap: () {
              ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(
                  content: Text(name, style: AppFonts.cairo()),
                  action: SnackBarAction(
                    label: 'الاتجاهات',
                    onPressed: () => widget.onOpenDirections(lat, lon),
                  ),
                ),
              );
            },
          ),
        ),
      );
    }

    _markers = markers;
  }

  void _onMapReady() {
    _mapReady = true;
    if (_pendingFit || widget.mapMosques.isNotEmpty) {
      _fitToMosques();
    }
  }

  void _fitToMosques() {
    if (!_mapReady || !mounted) return;

    final position = widget.position;
    if (position == null || widget.mapMosques.isEmpty) {
      _pendingFit = false;
      return;
    }

    _pendingFit = false;
    _ignoreMapEvents = true;
    final points = <LatLng>[
      LatLng(position.latitude, position.longitude),
      ...widget.mapMosques.map((m) => LatLng(m['lat'] as double, m['lon'] as double)),
    ];
    try {
      _mapController.fitCamera(
        CameraFit.bounds(
          bounds: LatLngBounds.fromPoints(points),
          padding: const EdgeInsets.all(48),
        ),
      );
    } catch (e) {
      debugPrint('fitCamera skipped: $e');
    }
    Future.delayed(const Duration(milliseconds: 600), () {
      if (mounted) _ignoreMapEvents = false;
    });
  }

  void _onMapEvent(MapEvent event) {
    if (!_mapReady || _ignoreMapEvents || event is! MapEventMoveEnd) return;

    final zoom = _mapController.camera.zoom;
    final bucket = zoom.floor();

    _zoomDebounce?.cancel();
    _zoomDebounce = Timer(const Duration(milliseconds: 700), () {
      if (!mounted) return;
      if ((bucket - _lastZoomBucket).abs() < 1) return;
      _lastZoomBucket = bucket.toDouble();
      widget.onZoomSettled(zoom);
    });
  }

  @override
  Widget build(BuildContext context) {
    final position = widget.position;
    final showHint =
        widget.isLoadingMore || widget.totalMosques > widget.mapMosques.length;

    return RepaintBoundary(
      child: Stack(
        children: [
          FlutterMap(
            mapController: _mapController,
            options: MapOptions(
              initialCenter: position != null
                  ? LatLng(position.latitude, position.longitude)
                  : const LatLng(29.3759, 47.9774),
              initialZoom: 14,
              interactionOptions: const InteractionOptions(flags: InteractiveFlag.all),
              onMapReady: _onMapReady,
              onMapEvent: _onMapEvent,
            ),
            children: [
              TileLayer(
                urlTemplate: widget.tileUrl,
                subdomains: _cartoSubdomains,
                userAgentPackageName: 'com.masged.parent_app',
                maxZoom: 18,
                keepBuffer: 1,
              ),
              MarkerLayer(markers: _markers),
            ],
          ),
          if (showHint)
            Positioned(
              top: 12,
              left: 12,
              right: 12,
              child: Material(
                elevation: 2,
                borderRadius: BorderRadius.circular(12),
                color: Colors.white.withValues(alpha: 0.95),
                child: Padding(
                  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                  child: Row(
                    children: [
                      if (widget.isLoadingMore)
                        const Padding(
                          padding: EdgeInsets.only(left: 8),
                          child: SizedBox(
                            width: 16,
                            height: 16,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          ),
                        ),
                      Expanded(
                        child: Text(
                          widget.isLoadingMore
                              ? 'جاري تحميل المزيد...'
                              : 'عرض ${widget.mapMosques.length} من ${widget.totalMosques} — بعّد الخريطة لعرض المزيد',
                          style: AppFonts.cairo(fontSize: 11, color: AppColors.textSecondary),
                          textAlign: TextAlign.center,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}

/// Lightweight pin — no GoogleFonts, fixed layout (no overflow).
class _MosqueMapPin extends StatelessWidget {
  const _MosqueMapPin({required this.name, required this.onTap});

  final String name;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      behavior: HitTestBehavior.opaque,
      child: SizedBox(
        width: 88,
        height: 40,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.end,
          children: [
            DecoratedBox(
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(6),
                border: Border.all(color: AppColors.primary.withValues(alpha: 0.35)),
                boxShadow: const [BoxShadow(color: Colors.black12, blurRadius: 3)],
              ),
              child: SizedBox(
                height: 14,
                width: 84,
                child: Center(
                  child: Text(
                    name,
                    style: _markerLabelStyle,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    textAlign: TextAlign.center,
                  ),
                ),
              ),
            ),
            const Icon(Icons.mosque_rounded, color: AppColors.primary, size: 18),
            const Icon(Icons.arrow_drop_down, color: AppColors.primary, size: 8),
          ],
        ),
      ),
    );
  }
}
