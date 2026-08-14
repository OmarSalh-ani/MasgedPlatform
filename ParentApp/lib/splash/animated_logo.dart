import 'dart:math' as math;

import 'package:flutter/material.dart';

import 'splash_colors.dart';

/// Asset path for the mosque logo — kept exactly as provided.
const String kSplashLogoAsset = 'assets/images/white_logo.png';

/// Animated mosque logo with fade/scale entrance, star twinkle overlays,
/// and a soft pulsing light positioned behind the blue icon.
class AnimatedLogo extends StatelessWidget {
  const AnimatedLogo({
    super.key,
    required this.fade,
    required this.scale,
    required this.starTwinkle,
    required this.starEntrance,
    required this.lightScale,
    required this.lightOpacity,
  });

  /// Logo opacity (0 → 1), 700 ms easeOutCubic.
  final double fade;

  /// Logo scale (0.85 → 1.0), 900 ms easeOutBack.
  final double scale;

  /// Repeating twinkle phase for gold stars (0 → 1 every 2 s).
  final double starTwinkle;

  /// One-shot entrance for star overlays (0 → 1).
  final double starEntrance;

  /// Radial light scale behind icon (0.95 → 1.05).
  final double lightScale;

  /// Radial light opacity (0.05 → 0.08).
  final double lightOpacity;

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final maxWidth = constraints.maxWidth;
        final logoWidth = math.min(maxWidth * 0.62, 300.0);

        return Opacity(
          opacity: fade.clamp(0.0, 1.0),
          child: Transform.scale(
            scale: scale,
            child: SizedBox(
              width: logoWidth,
              child: AspectRatio(
                aspectRatio: 0.82,
                child: Stack(
                  alignment: Alignment.center,
                  clipBehavior: Clip.none,
                  children: [
                    // Soft light pulse behind the blue dome icon.
                    Positioned(
                      top: logoWidth * 0.14,
                      child: Transform.scale(
                        scale: lightScale,
                        child: Container(
                          width: logoWidth * 0.52,
                          height: logoWidth * 0.38,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            gradient: RadialGradient(
                              colors: [
                                SplashColors.lightRay(lightOpacity),
                                Colors.transparent,
                              ],
                            ),
                          ),
                        ),
                      ),
                    ),

                    // Full logo — icon and Arabic text unchanged.
                    Positioned.fill(
                      child: Image.asset(
                        kSplashLogoAsset,
                        fit: BoxFit.contain,
                        filterQuality: FilterQuality.high,
                        gaplessPlayback: true,
                      ),
                    ),

                    // Soft gold glow overlays — twinkle without redrawing logo stars.
                    Positioned(
                      top: logoWidth * 0.062,
                      child: _StarGlowOverlay(
                        size: logoWidth * 0.07,
                        entrance: starEntrance,
                        twinkle: starTwinkle,
                      ),
                    ),
                    Positioned(
                      top: logoWidth * 0.112,
                      child: _StarGlowOverlay(
                        size: logoWidth * 0.09,
                        entrance: starEntrance,
                        twinkle: starTwinkle,
                        twinklePhaseOffset: 0.35,
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        );
      },
    );
  }
}

/// Opacity + scale twinkle glow aligned with logo star positions.
class _StarGlowOverlay extends StatelessWidget {
  const _StarGlowOverlay({
    required this.size,
    required this.entrance,
    required this.twinkle,
    this.twinklePhaseOffset = 0.0,
  });

  final double size;
  final double entrance;
  final double twinkle;
  final double twinklePhaseOffset;

  @override
  Widget build(BuildContext context) {
    final phase = (twinkle + twinklePhaseOffset) % 1.0;
    final twinkleScale = 0.94 + 0.06 * math.sin(phase * math.pi * 2);
    final twinkleOpacity = 0.35 + 0.65 * math.sin(phase * math.pi * 2);

    return IgnorePointer(
      child: Opacity(
        opacity: (entrance * twinkleOpacity * 0.55).clamp(0.0, 1.0),
        child: Transform.scale(
          scale: (0.88 + entrance * 0.12) * twinkleScale,
          child: Container(
            width: size,
            height: size,
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              gradient: RadialGradient(
                colors: [
                  SplashColors.gold.withValues(alpha: 0.55),
                  SplashColors.gold.withValues(alpha: 0.12),
                  Colors.transparent,
                ],
                stops: const [0.0, 0.35, 1.0],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
