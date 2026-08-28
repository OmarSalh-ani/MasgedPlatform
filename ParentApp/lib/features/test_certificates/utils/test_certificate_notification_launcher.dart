import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../shared/router/app_router.dart';
import '../../../shared/router/app_routes.dart';

Future<void> openTestCertificateFromPushNotification(
  WidgetRef ref, {
  int? testId,
  int? studentId,
}) async {
  final context =
      ref.read(appRouterProvider).routerDelegate.navigatorKey.currentContext;
  if (context == null || !context.mounted) return;

  context.push(
    AppRoutes.testCertificatesPath(
      studentId: studentId,
      testId: testId,
    ),
  );
}
