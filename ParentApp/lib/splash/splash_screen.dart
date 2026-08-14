import 'package:flutter/material.dart';

import 'animated_logo.dart';
import 'floating_particles.dart';
import 'light_background.dart';
import 'splash_colors.dart';

/// Exit fade duration — shared with [navigateWithFadeTransition].
const Duration kSplashExitFadeDuration = Duration(milliseconds: 500);

/// Premium animated splash screen with restrained, elegant motion.
///
/// Timeline:
/// - 0–700 ms: logo fade in (easeOutCubic)
/// - 0–900 ms: logo scale 0.85 → 1.0 (easeOutBack)
/// - Stars fade/scale in, then twinkle every 2 s (opacity + scale only)
/// - Light ray pulses slowly behind the icon (5–8% opacity)
/// - Particles drift upward at 8% opacity
/// - 2800 ms: exit fade begins (500 ms)
class SplashScreen extends StatefulWidget {
  const SplashScreen({
    super.key,
    required this.onComplete,
  });

  /// Called after the exit fade finishes — use for navigation.
  final VoidCallback onComplete;

  @override
  State<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends State<SplashScreen>
    with TickerProviderStateMixin {
  static const _logoFadeDuration = Duration(milliseconds: 700);
  static const _logoScaleDuration = Duration(milliseconds: 900);
  static const _starTwinklePeriod = Duration(seconds: 2);
  static const _lightPulseDuration = Duration(milliseconds: 4200);
  static const _particleDuration = Duration(milliseconds: 18000);
  static const _holdDuration = Duration(milliseconds: 2800);
  static const _exitFadeDuration = kSplashExitFadeDuration;

  late final AnimationController _logoFadeController;
  late final AnimationController _logoScaleController;
  late final AnimationController _starTwinkleController;
  late final AnimationController _starEntranceController;
  late final AnimationController _lightController;
  late final AnimationController _particleController;
  late final AnimationController _exitFadeController;

  late final Animation<double> _logoFade;
  late final Animation<double> _logoScale;
  late final Animation<double> _starEntrance;
  late final Animation<double> _lightScale;
  late final Animation<double> _lightOpacity;

  @override
  void initState() {
    super.initState();
    _initControllers();
    _startSequence();
  }

  void _initControllers() {
    _logoFadeController = AnimationController(
      vsync: this,
      duration: _logoFadeDuration,
    );
    _logoFade = CurvedAnimation(
      parent: _logoFadeController,
      curve: Curves.easeOutCubic,
    );

    _logoScaleController = AnimationController(
      vsync: this,
      duration: _logoScaleDuration,
    );
    _logoScale = CurvedAnimation(
      parent: _logoScaleController,
      curve: Curves.easeOutBack,
    ).drive(Tween<double>(begin: 0.85, end: 1.0));

    _starEntranceController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 800),
    );
    _starEntrance = CurvedAnimation(
      parent: _starEntranceController,
      curve: Curves.easeOutCubic,
    );

    _starTwinkleController = AnimationController(
      vsync: this,
      duration: _starTwinklePeriod,
    );

    _lightController = AnimationController(
      vsync: this,
      duration: _lightPulseDuration,
    );
    _lightScale = Tween<double>(begin: 0.95, end: 1.05).animate(
      CurvedAnimation(parent: _lightController, curve: Curves.easeInOut),
    );
    _lightOpacity = Tween<double>(begin: 0.05, end: 0.08).animate(
      CurvedAnimation(parent: _lightController, curve: Curves.easeInOut),
    );

    _particleController = AnimationController(
      vsync: this,
      duration: _particleDuration,
    );

    _exitFadeController = AnimationController(
      vsync: this,
      duration: _exitFadeDuration,
    );
  }

  Future<void> _startSequence() async {
    _logoFadeController.forward();
    _logoScaleController.forward();
    _starEntranceController.forward();

    await Future<void>.delayed(const Duration(milliseconds: 400));
    if (!mounted) return;

    _starTwinkleController.repeat();
    _lightController.repeat(reverse: true);
    _particleController.repeat();

    await Future<void>.delayed(_holdDuration);
    if (!mounted) return;

    await _exitFadeController.forward();
    if (!mounted) return;

    widget.onComplete();
  }

  @override
  void dispose() {
    _logoFadeController.dispose();
    _logoScaleController.dispose();
    _starTwinkleController.dispose();
    _starEntranceController.dispose();
    _lightController.dispose();
    _particleController.dispose();
    _exitFadeController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: SplashColors.background,
      body: AnimatedBuilder(
        animation: Listenable.merge([
          _logoFade,
          _logoScale,
          _starTwinkleController,
          _starEntrance,
          _lightController,
          _particleController,
          _exitFadeController,
        ]),
        builder: (context, _) {
          final exitOpacity = 1.0 - _exitFadeController.value;

          return Opacity(
            opacity: exitOpacity.clamp(0.0, 1.0),
            child: Stack(
              fit: StackFit.expand,
              children: [
                LightBackground(
                  lightScale: _lightScale.value,
                  lightOpacity: _lightOpacity.value,
                ),
                FloatingParticles(progress: _particleController.value),
                SafeArea(
                  child: Center(
                    child: Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 32),
                      child: AnimatedLogo(
                        fade: _logoFade.value,
                        scale: _logoScale.value,
                        starTwinkle: _starTwinkleController.value,
                        starEntrance: _starEntrance.value,
                        lightScale: _lightScale.value,
                        lightOpacity: _lightOpacity.value,
                      ),
                    ),
                  ),
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}

/// Navigates to [destination] with a 500 ms fade transition.
Future<void> navigateWithFadeTransition(
  BuildContext context,
  Widget destination,
) {
  return Navigator.of(context).pushReplacement(
    PageRouteBuilder<void>(
      transitionDuration: kSplashExitFadeDuration,
      reverseTransitionDuration: kSplashExitFadeDuration,
      pageBuilder: (context, animation, secondaryAnimation) => destination,
      transitionsBuilder: (context, animation, secondaryAnimation, child) {
        return FadeTransition(
          opacity: CurvedAnimation(
            parent: animation,
            curve: Curves.easeInOut,
          ),
          child: child,
        );
      },
    ),
  );
}
