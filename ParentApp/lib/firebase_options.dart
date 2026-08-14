// Run `dart pub global activate flutterfire_cli` then `flutterfire configure`
// to replace this file with your real Firebase project values.
import 'package:firebase_core/firebase_core.dart' show FirebaseOptions;
import 'package:flutter/foundation.dart'
    show defaultTargetPlatform, kIsWeb, TargetPlatform;

class DefaultFirebaseOptions {
  static FirebaseOptions get currentPlatform {
    if (kIsWeb) {
      return web;
    }
    switch (defaultTargetPlatform) {
      case TargetPlatform.android:
        return android;
      case TargetPlatform.iOS:
        return ios;
      default:
        return android;
    }
  }

  static const FirebaseOptions web = FirebaseOptions(
    apiKey: 'AIzaSyBC2ffdoqtIN4kn7IXtNuJFGzveOgikwm8',
    appId: '1:438891431699:web:f8b78332e6cb0548f6bce6',
    messagingSenderId: '438891431699',
    projectId: 'mubarak-masged-mobile',
    authDomain: 'mubarak-masged-mobile.firebaseapp.com',
    storageBucket: 'mubarak-masged-mobile.firebasestorage.app',
  );

  static const FirebaseOptions android = FirebaseOptions(
    apiKey: 'AIzaSyAvH8FqibnX6l3sVdu3X_QJM9LA4pIM5CI',
    appId: '1:438891431699:android:4a7f93d073c40dacf6bce6',
    messagingSenderId: '438891431699',
    projectId: 'mubarak-masged-mobile',
    storageBucket: 'mubarak-masged-mobile.firebasestorage.app',
  );

  static const FirebaseOptions ios = FirebaseOptions(
    apiKey: 'AIzaSyCb_pGkiXzg3Q9x7zy7ZFI9F6fS_xote0Q',
    appId: '1:438891431699:ios:23ab917335e57ee9f6bce6',
    messagingSenderId: '438891431699',
    projectId: 'mubarak-masged-mobile',
    storageBucket: 'mubarak-masged-mobile.firebasestorage.app',
    iosBundleId: 'com.mubarakmasged.com',
  );

}