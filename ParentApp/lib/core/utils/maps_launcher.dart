import 'package:flutter/foundation.dart';
import 'package:url_launcher/url_launcher.dart';

/// Opens directions/navigation to [lat],[lon] in an external maps app.
class MapsLauncher {
  MapsLauncher._();

  static Future<bool> openDirections({
    required double lat,
    required double lon,
  }) async {
    final query = '$lat,$lon';
    final candidates = <Uri>[
      Uri.parse('google.navigation:q=$query'),
      Uri.parse('comgooglemaps://?q=$query&directionsmode=driving'),
      Uri.parse('geo:$query?q=$query'),
      Uri.parse('https://www.google.com/maps/dir/?api=1&destination=$query'),
      Uri.parse('https://www.google.com/maps/search/?api=1&query=$query'),
    ];

    for (final uri in candidates) {
      try {
        final launched = await launchUrl(
          uri,
          mode: LaunchMode.externalApplication,
        );
        if (launched) return true;
      } catch (e) {
        debugPrint('MapsLauncher: could not open $uri — $e');
      }
    }
    return false;
  }
}
