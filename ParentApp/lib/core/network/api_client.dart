import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../constants/app_constants.dart';
import 'api_exception.dart';

class ApiClient {
  ApiClient._();

  static final ApiClient instance = ApiClient._();

  late final Dio _dio = () {
    final dio = Dio(
      BaseOptions(
        baseUrl: AppConstants.apiBaseUrl,
        connectTimeout: const Duration(seconds: 30),
        receiveTimeout: const Duration(seconds: 30),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
      ),
    );

    dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final prefs = await SharedPreferences.getInstance();
          final token = prefs.getString(AppConstants.keyAuthToken);
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (error, handler) {
          handler.reject(
            DioException(
              requestOptions: error.requestOptions,
              response: error.response,
              type: error.type,
              error: ApiException.fromDioException(error),
            ),
          );
        },
      ),
    );

    if (kDebugMode) {
      dio.interceptors.add(
        LogInterceptor(
          requestHeader: true,
          requestBody: true,
          responseHeader: false,
          responseBody: true,
          error: true,
          logPrint: (obj) => debugPrint(obj.toString()),
        ),
      );
    }

    return dio;
  }();

  Dio get dio => _dio;

  /// GET that returns raw text (e.g. HTML certificate).
  Future<String> getText(
    String path, {
    Map<String, dynamic>? queryParameters,
    String accept = 'text/html',
  }) async {
    try {
      final response = await _dio.get<String>(
        path,
        queryParameters: queryParameters,
        options: Options(
          responseType: ResponseType.plain,
          headers: {'Accept': accept},
        ),
      );
      final text = response.data;
      if (text == null || text.trim().isEmpty) {
        throw ApiException('استجابة فارغة من الخادم');
      }
      return text;
    } on DioException catch (e) {
      if (e.error is ApiException) throw e.error as ApiException;
      throw ApiException.fromDioException(e);
    }
  }

  /// GET that returns raw file bytes (PDF exports).
  Future<({List<int> bytes, String? fileName})> getBytes(
    String path, {
    Map<String, dynamic>? queryParameters,
  }) async {
    try {
      final response = await _dio.get<List<int>>(
        path,
        queryParameters: queryParameters,
        options: Options(
          responseType: ResponseType.bytes,
          headers: {'Accept': 'application/pdf'},
        ),
      );
      final bytes = response.data;
      if (bytes == null || bytes.isEmpty) {
        throw ApiException('استجابة فارغة من الخادم');
      }

      String? fileName;
      final disposition = response.headers.value('content-disposition');
      if (disposition != null && disposition.isNotEmpty) {
        final utf8Match =
            RegExp(r"filename\*=UTF-8''([^;]+)").firstMatch(disposition);
        final plainMatch =
            RegExp(r'filename="?([^";]+)"?').firstMatch(disposition);
        final raw = utf8Match?.group(1) ?? plainMatch?.group(1);
        if (raw != null && raw.isNotEmpty) {
          fileName = Uri.decodeComponent(raw.trim());
        }
      }

      return (bytes: bytes, fileName: fileName);
    } on DioException catch (e) {
      if (e.error is ApiException) throw e.error as ApiException;
      throw ApiException.fromDioException(e);
    }
  }
}
