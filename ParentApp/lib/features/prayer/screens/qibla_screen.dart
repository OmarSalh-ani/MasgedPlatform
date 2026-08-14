import 'dart:async';
import 'dart:math' show cos, min, pi, sin;

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:masged_parent_app/core/theme/app_fonts.dart';
import 'package:flutter_compass_v2/flutter_compass_v2.dart';
import 'package:flutter_qiblah/flutter_qiblah.dart';
import 'package:geolocator/geolocator.dart';
import 'package:go_router/go_router.dart';

import '../../../core/theme/app_colors.dart';
import '../widgets/location_error_widget.dart';

class QiblaScreen extends StatefulWidget {
  const QiblaScreen({super.key});

  @override
  State<QiblaScreen> createState() => _QiblaScreenState();
}

class _QiblaScreenState extends State<QiblaScreen> {
  /// Cached once — do not create inside [build] or the loader never finishes.
  final Future<bool?> _deviceSupport = FlutterQiblah.androidDeviceSensorSupport();

  LocationStatus? _locationStatus;
  bool _locationLoading = true;

  @override
  void initState() {
    super.initState();
    _checkLocationStatus();
  }

  @override
  void dispose() {
    FlutterQiblah().dispose();
    super.dispose();
  }

  Future<void> _checkLocationStatus() async {
    setState(() => _locationLoading = true);

    var locationStatus = await FlutterQiblah.checkLocationStatus();

    if (locationStatus.status == LocationPermission.denied) {
      await FlutterQiblah.requestPermissions();
      locationStatus = await FlutterQiblah.checkLocationStatus();
    }

    if (!mounted) return;
    setState(() {
      _locationStatus = locationStatus;
      _locationLoading = false;
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
            'اتجاه القبلة',
            style: AppFonts.cairo(fontWeight: FontWeight.bold),
          ),
          leading: IconButton(
            icon: const Icon(Icons.arrow_back_ios_new_rounded,
                color: AppColors.textPrimary),
            onPressed: () => context.pop(),
          ),
        ),
        body: FutureBuilder<bool?>(
          future: _deviceSupport,
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            }
            if (snapshot.hasError) {
              return Center(child: Text('خطأ: ${snapshot.error}'));
            }

            final sensorSupported = snapshot.data ?? false;
            if (!sensorSupported) {
              return Center(
                child: Padding(
                  padding: const EdgeInsets.all(32.0),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(Icons.sensors_off_rounded,
                          size: 64, color: Colors.grey),
                      const SizedBox(height: 16),
                      Text(
                        'جهازك لا يدعم مستشعر البوصلة',
                        style: AppFonts.cairo(
                            fontSize: 18, color: Colors.grey[700]),
                        textAlign: TextAlign.center,
                      ),
                    ],
                  ),
                ),
              );
            }

            if (_locationLoading || _locationStatus == null) {
              return const Center(child: CupertinoActivityIndicator());
            }

            final locationStatus = _locationStatus!;

            if (locationStatus.enabled) {
              switch (locationStatus.status) {
                case LocationPermission.always:
                case LocationPermission.whileInUse:
                  return const QiblahCompassWidget();
                case LocationPermission.denied:
                  return LocationErrorWidget(
                    error: 'تم رفض الإذن بالوصول للموقع',
                    callback: _checkLocationStatus,
                  );
                case LocationPermission.deniedForever:
                  return LocationErrorWidget(
                    error: 'تم رفض الإذن بالوصول للموقع بشكل دائم',
                    callback: _checkLocationStatus,
                  );
                default:
                  return LocationErrorWidget(
                    error: 'تعذر تحديد حالة الموقع',
                    callback: _checkLocationStatus,
                  );
              }
            }

            return LocationErrorWidget(
              error: 'يرجى تفعيل خدمة الموقع',
              callback: _checkLocationStatus,
            );
          },
        ),
      ),
    );
  }
}

class QiblahCompassWidget extends StatefulWidget {
  const QiblahCompassWidget({super.key});

  @override
  State<QiblahCompassWidget> createState() => _QiblahCompassWidgetState();
}

class _QiblahCompassWidgetState extends State<QiblahCompassWidget> {
  String? _setupError;
  bool _needsCalibration = false;
  StreamSubscription<CompassEvent>? _compassSubscription;

  @override
  void initState() {
    super.initState();
    _primeLocation();
    _compassSubscription = FlutterCompass.events?.listen((event) {
      final needsCalibration = event.accuracy == null;
      if (needsCalibration != _needsCalibration && mounted) {
        setState(() => _needsCalibration = needsCalibration);
      }
    });
  }

  @override
  void dispose() {
    _compassSubscription?.cancel();
    super.dispose();
  }

  /// Warm up GPS so [FlutterQiblah.qiblahStream] can combine compass + position.
  Future<void> _primeLocation() async {
    try {
      await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.medium,
          timeLimit: Duration(seconds: 15),
        ),
      );
    } catch (e) {
      if (mounted) {
        setState(() => _setupError = e.toString());
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_setupError != null) {
      return Center(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Text(
            'تعذر الحصول على الموقع: $_setupError',
            textAlign: TextAlign.center,
            style: AppFonts.cairo(color: Colors.red.shade700),
          ),
        ),
      );
    }

    return StreamBuilder<QiblahDirection>(
      stream: FlutterQiblah.qiblahStream,
      builder: (_, AsyncSnapshot<QiblahDirection> snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        }

        if (snapshot.hasError) {
          return Center(child: Text('خطأ: ${snapshot.error}'));
        }

        if (!snapshot.hasData) {
          return Center(
            child: Text(
              'لا توجد بيانات من البوصلة.\nجرّب على جهاز حقيقي وليس المحاكي.',
              textAlign: TextAlign.center,
              style: AppFonts.cairo(color: AppColors.textSecondary),
            ),
          );
        }

        final data = snapshot.data!;
        final isAligned = _isFacingQibla(data.qiblah);
        final deviation = _deviationFromQibla(data.qiblah);

        return SafeArea(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
            child: Column(
              children: [
                if (_needsCalibration) _CalibrationBanner(),
                const SizedBox(height: 8),
                Text(
                  'أمسك الهاتف بشكل مسطح وحرّكه ببطء',
                  textAlign: TextAlign.center,
                  style: AppFonts.cairo(
                    fontSize: 14,
                    color: AppColors.textSecondary,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'السهم يشير إلى القبلة — اجعله للأعلى عندما تكون في الاتجاه الصحيح',
                  textAlign: TextAlign.center,
                  style: AppFonts.cairo(
                    fontSize: 13,
                    color: AppColors.textHint,
                  ),
                ),
                const SizedBox(height: 24),
                Expanded(
                  child: Center(
                    child: LayoutBuilder(
                      builder: (context, constraints) {
                        final size = min(
                          min(constraints.maxWidth, constraints.maxHeight),
                          300.0,
                        );
                        return _QiblaCompassDial(
                          size: size,
                          direction: data.direction,
                          qiblah: data.qiblah,
                          isAligned: isAligned,
                          needsCalibration: _needsCalibration,
                        );
                      },
                    ),
                  ),
                ),
                const SizedBox(height: 24),
                _QiblaStatusCard(
                  isAligned: isAligned,
                  deviation: deviation,
                  offsetFromNorth: data.offset,
                  needsCalibration: _needsCalibration,
                ),
              ],
            ),
          ),
        );
      },
    );
  }
}

/// True when the qibla arrow points to the top of the screen (facing qibla).
bool _isFacingQibla(double qiblah) => _deviationFromQibla(qiblah) < 5;

double _deviationFromQibla(double qiblah) {
  final normalized = qiblah % 360;
  return min(normalized, 360 - normalized);
}

class _CalibrationBanner extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.orange.shade50,
      borderRadius: BorderRadius.circular(12),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Row(
          children: [
            Icon(Icons.compass_calibration_rounded,
                color: Colors.orange.shade800),
            const SizedBox(width: 12),
            Expanded(
              child: Text(
                'البوصلة تحتاج معايرة. حرّك الهاتف بحركة رقم 8 في الهواء.',
                style: AppFonts.cairo(
                  color: Colors.orange.shade900,
                  fontSize: 13,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _QiblaCompassDial extends StatelessWidget {
  const _QiblaCompassDial({
    required this.size,
    required this.direction,
    required this.qiblah,
    required this.isAligned,
    required this.needsCalibration,
  });

  final double size;
  final double direction;
  final double qiblah;
  final bool isAligned;
  final bool needsCalibration;

  @override
  Widget build(BuildContext context) {
    final arrowColor = isAligned
        ? AppColors.success
        : (needsCalibration ? Colors.orange : AppColors.primary);

    return SizedBox(
      width: size,
      height: size,
      child: Stack(
        alignment: Alignment.center,
        children: [
          // Fixed outer ring — top marker = direction the phone is facing
          CustomPaint(
            size: Size(size, size),
            painter: _FixedRingPainter(isAligned: isAligned),
          ),
          // Compass rose rotates with magnetic north
          Transform.rotate(
            angle: -direction * (pi / 180),
            child: CustomPaint(
              size: Size(size * 0.88, size * 0.88),
              painter: _CompassRosePainter(),
            ),
          ),
          // Qibla arrow — uses [qiblah], not [offset] (offset is fixed per location)
          Transform.rotate(
            angle: -qiblah * (pi / 180),
            child: CustomPaint(
              size: Size(size * 0.55, size * 0.55),
              painter: _QiblaArrowPainter(color: arrowColor),
            ),
          ),
          Container(
            width: 14,
            height: 14,
            decoration: BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
              border: Border.all(color: AppColors.border, width: 2),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withValues(alpha: 0.08),
                  blurRadius: 4,
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _QiblaStatusCard extends StatelessWidget {
  const _QiblaStatusCard({
    required this.isAligned,
    required this.deviation,
    required this.offsetFromNorth,
    required this.needsCalibration,
  });

  final bool isAligned;
  final double deviation;
  final double offsetFromNorth;
  final bool needsCalibration;

  @override
  Widget build(BuildContext context) {
    final statusColor =
        isAligned ? AppColors.success : AppColors.textSecondary;
    final statusIcon = isAligned
        ? Icons.check_circle_rounded
        : Icons.explore_rounded;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(
          color: isAligned ? AppColors.success.withValues(alpha: 0.4) : AppColors.border,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withValues(alpha: 0.04),
            blurRadius: 12,
            offset: const Offset(0, 4),
          ),
        ],
      ),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(statusIcon, color: statusColor, size: 28),
              const SizedBox(width: 10),
              Text(
                isAligned ? 'أنت في اتجاه القبلة' : 'استدر حتى يشير السهم للأعلى',
                style: AppFonts.cairo(
                  fontSize: 17,
                  fontWeight: FontWeight.bold,
                  color: statusColor,
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              _StatChip(
                label: 'انحرافك عن القبلة',
                value: '${deviation.toStringAsFixed(0)}°',
              ),
              _StatChip(
                label: 'زاوية القبلة من الشمال',
                value: '${offsetFromNorth.toStringAsFixed(0)}°',
              ),
            ],
          ),
          if (needsCalibration && !isAligned) ...[
            const SizedBox(height: 10),
            Text(
              'دقة البوصلة منخفضة — قد يتحرك السهم ببطء',
              textAlign: TextAlign.center,
              style: AppFonts.cairo(
                fontSize: 12,
                color: Colors.orange.shade800,
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _StatChip extends StatelessWidget {
  const _StatChip({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          value,
          style: AppFonts.cairo(
            fontSize: 22,
            fontWeight: FontWeight.bold,
            color: AppColors.primary,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          label,
          style: AppFonts.cairo(
            fontSize: 12,
            color: AppColors.textSecondary,
          ),
        ),
      ],
    );
  }
}

class _FixedRingPainter extends CustomPainter {
  _FixedRingPainter({required this.isAligned});

  final bool isAligned;

  @override
  void paint(Canvas canvas, Size size) {
    final center = Offset(size.width / 2, size.height / 2);
    final radius = size.width / 2 - 4;

    final ringPaint = Paint()
      ..color = isAligned ? AppColors.successLight : AppColors.primaryLight
      ..style = PaintingStyle.stroke
      ..strokeWidth = 6;
    canvas.drawCircle(center, radius, ringPaint);

    final borderPaint = Paint()
      ..color = AppColors.border
      ..style = PaintingStyle.stroke
      ..strokeWidth = 1.5;
    canvas.drawCircle(center, radius, borderPaint);

    // Top marker = phone forward direction
    final markerPath = Path()
      ..moveTo(center.dx, center.dy - radius + 8)
      ..lineTo(center.dx - 10, center.dy - radius + 28)
      ..lineTo(center.dx + 10, center.dy - radius + 28)
      ..close();
    final markerPaint = Paint()..color = AppColors.textPrimary;
    canvas.drawPath(markerPath, markerPaint);

    final textPainter = TextPainter(
      text: TextSpan(
        text: 'أمامك',
        style: TextStyle(
          color: AppColors.textSecondary,
          fontSize: 11,
          fontWeight: FontWeight.w600,
        ),
      ),
      textDirection: TextDirection.rtl,
    )..layout();
    textPainter.paint(
      canvas,
      Offset(center.dx - textPainter.width / 2, center.dy - radius - 22),
    );
  }

  @override
  bool shouldRepaint(covariant _FixedRingPainter oldDelegate) =>
      oldDelegate.isAligned != isAligned;
}

class _CompassRosePainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final center = Offset(size.width / 2, size.height / 2);
    final radius = size.width / 2 - 8;

    final bgPaint = Paint()
      ..color = Colors.white
      ..style = PaintingStyle.fill;
    canvas.drawCircle(center, radius, bgPaint);

    final tickPaint = Paint()
      ..color = AppColors.border
      ..strokeWidth = 1;
    for (var i = 0; i < 72; i++) {
      final angle = i * 5 * pi / 180;
      final inner = i % 9 == 0 ? radius - 18 : radius - 10;
      final outer = radius - 4;
      canvas.drawLine(
        Offset(center.dx + inner * sin(angle), center.dy - inner * cos(angle)),
        Offset(center.dx + outer * sin(angle), center.dy - outer * cos(angle)),
        tickPaint,
      );
    }

    _drawCardinal(canvas, center, radius, 'ش', 0, AppColors.error);
    _drawCardinal(canvas, center, radius, 'ق', 90, AppColors.primary);
    _drawCardinal(canvas, center, radius, 'ج', 180, AppColors.textSecondary);
    _drawCardinal(canvas, center, radius, 'غ', 270, AppColors.textSecondary);
  }

  void _drawCardinal(
    Canvas canvas,
    Offset center,
    double radius,
    String label,
    double degrees,
    Color color,
  ) {
    final angle = degrees * pi / 180;
    final pos = Offset(
      center.dx + (radius - 28) * sin(angle),
      center.dy - (radius - 28) * cos(angle),
    );
    final tp = TextPainter(
      text: TextSpan(
        text: label,
        style: TextStyle(
          color: color,
          fontSize: degrees == 0 ? 16 : 13,
          fontWeight: FontWeight.bold,
        ),
      ),
      textDirection: TextDirection.rtl,
    )..layout();
    tp.paint(canvas, pos - Offset(tp.width / 2, tp.height / 2));
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _QiblaArrowPainter extends CustomPainter {
  _QiblaArrowPainter({required this.color});

  final Color color;

  @override
  void paint(Canvas canvas, Size size) {
    final center = Offset(size.width / 2, size.height / 2);
    final h = size.height / 2;

    final shadowPaint = Paint()
      ..color = color.withValues(alpha: 0.25)
      ..maskFilter = const MaskFilter.blur(BlurStyle.normal, 6);
    final arrowPath = Path()
      ..moveTo(center.dx, center.dy - h + 8)
      ..lineTo(center.dx - 22, center.dy + h * 0.35)
      ..lineTo(center.dx, center.dy + h * 0.1)
      ..lineTo(center.dx + 22, center.dy + h * 0.35)
      ..close();
    canvas.drawPath(arrowPath, shadowPaint);

    final arrowPaint = Paint()..color = color;
    canvas.drawPath(arrowPath, arrowPaint);

    // Kaaba hint at arrow base
    final base = RRect.fromRectAndRadius(
      Rect.fromCenter(
        center: Offset(center.dx, center.dy + h * 0.55),
        width: 20,
        height: 20,
      ),
      const Radius.circular(4),
    );
    canvas.drawRRect(
      base,
      Paint()..color = AppColors.gold,
    );
  }

  @override
  bool shouldRepaint(covariant _QiblaArrowPainter oldDelegate) =>
      oldDelegate.color != color;
}
