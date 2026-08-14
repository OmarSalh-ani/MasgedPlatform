import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:geolocator/geolocator.dart';

class MosqueService {
  static const String overpassUrl = 'https://overpass-api.de/api/interpreter';
  static const String _fallbackOverpassUrl =
      'https://overpass.kumi.systems/api/interpreter';
  static const Map<String, String> _overpassHeaders = {
    'User-Agent': 'MasgedParentApp/1.0 (masged-parent-app)',
    'Accept': 'application/json',
  };

  Future<List<Map<String, dynamic>>> getNearbyMosques(Position position, {double radius = 5000}) async {
    final String query = '''
      [out:json][timeout:25];
      (
        node["amenity"="place_of_worship"]["religion"="muslim"](around:$radius,${position.latitude},${position.longitude});
        node["building"="mosque"](around:$radius,${position.latitude},${position.longitude});
        way["amenity"="place_of_worship"]["religion"="muslim"](around:$radius,${position.latitude},${position.longitude});
        way["building"="mosque"](around:$radius,${position.latitude},${position.longitude});
        relation["amenity"="place_of_worship"]["religion"="muslim"](around:$radius,${position.latitude},${position.longitude});
        relation["building"="mosque"](around:$radius,${position.latitude},${position.longitude});
      );
      out center;
    ''';

    try {
      final response = await _postOverpass(query);

      if (response.statusCode == 200) {
        final data = json.decode(response.body);
        final List elements = data['elements'];

        List<Map<String, dynamic>> mosques = [];
        for (final e in elements) {
          final tags = e['tags'] ?? {};
          final lat = e['lat'] ?? e['center']?['lat'];
          final lon = e['lon'] ?? e['center']?['lon'];
          if (lat == null || lon == null) continue;

          final latDouble = (lat as num).toDouble();
          final lonDouble = (lon as num).toDouble();

          final distance = Geolocator.distanceBetween(
            position.latitude,
            position.longitude,
            latDouble,
            lonDouble,
          );

          // Try generic 'addr' or 'addr:full' first as requested
          String address = tags['addr'] ?? tags['addr:full'] ?? '';
          
          if (address.isEmpty) {
            final String street = tags['addr:street'] ?? '';
            final String houseNumber = tags['addr:housenumber'] ?? '';
            final String suburb = tags['addr:suburb'] ?? '';
            final String city = tags['addr:city'] ?? '';
            
            address = [houseNumber, street, suburb, city].where((s) => s.isNotEmpty).join(', ');
          }

          if (address.isEmpty) address = 'العنوان غير متوفر';

          mosques.add({
            'name': _tagString(tags['name:ar'] ?? tags['name']) ?? 'مسجد غير معروف',
            'address': address,
            'distance': distance,
            'lat': latDouble,
            'lon': lonDouble,
          });
        }

        // Sort by distance and cap to keep parsing/UI light on mobile
        mosques.sort((a, b) => (a['distance'] as double).compareTo(b['distance'] as double));
        if (mosques.length > 80) {
          mosques = mosques.sublist(0, 80);
        }

        return mosques;
      } else {
        throw Exception('Overpass HTTP ${response.statusCode}');
      }
    } catch (e) {
      print('Error fetching mosques: $e');
      rethrow;
    }
  }

  Future<http.Response> _postOverpass(String query) async {
    const timeout = Duration(seconds: 60);
    for (final url in [overpassUrl, _fallbackOverpassUrl]) {
      final response = await http
          .post(
            Uri.parse(url),
            headers: _overpassHeaders,
            body: {'data': query},
          )
          .timeout(timeout);
      if (response.statusCode == 200 || response.statusCode == 429) {
        return response;
      }
    }
    throw Exception('Overpass API unavailable');
  }

  String? _tagString(dynamic value) {
    if (value == null) return null;
    return value.toString().trim();
  }
}
