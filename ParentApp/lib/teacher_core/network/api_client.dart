import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';

import 'package:masged_parent_app/core/config/unified_api_config.dart';

import '../config/api_config.dart';
import '../storage/auth_storage.dart';
import 'api_exception.dart';
import 'global_response.dart';

class TeacherApiClient {
  TeacherApiClient(this._authStorage) {
    _dio = Dio(
      BaseOptions(
        baseUrl: UnifiedApiConfig.teacherBaseUrl,
        connectTimeout: ApiConfig.connectTimeout,
        receiveTimeout: ApiConfig.receiveTimeout,
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
      ),
    );

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final token = await _authStorage.getToken();
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
      ),
    );

    if (kDebugMode) {
      _dio.interceptors.add(
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
  }

  final AuthStorage _authStorage;
  late final Dio _dio;

  Dio get dio => _dio;

  String _resolvePath(String path) => UnifiedApiConfig.teacherPath(path);

  Future<T> post<T>(
    String path, {
    Map<String, dynamic>? body,
    required T Function(dynamic json) parseData,
  }) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        _resolvePath(path),
        data: body,
      );
      return _parseEnvelope(response.data, parseData);
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  Future<T> get<T>(
    String path, {
    Map<String, dynamic>? queryParameters,
    required T Function(dynamic json) parseData,
  }) async {
    try {
      final response = await _dio.get<Map<String, dynamic>>(
        _resolvePath(path),
        queryParameters: queryParameters,
      );
      return _parseEnvelope(response.data, parseData);
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  /// GET that returns raw text (e.g. HTML certificate).
  Future<String> getText(
    String path, {
    Map<String, dynamic>? queryParameters,
    String accept = 'text/html',
  }) async {
    try {
      final response = await _dio.get<String>(
        _resolvePath(path),
        queryParameters: queryParameters,
        options: Options(
          responseType: ResponseType.plain,
          headers: {'Accept': accept},
        ),
      );
      final text = response.data;
      if (text == null || text.trim().isEmpty) {
        throw ApiException(message: 'استجابة فارغة من الخادم');
      }
      return text;
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  /// GET that returns raw file bytes (PDF / Excel exports).
  Future<({List<int> bytes, String? fileName})> getBytes(
    String path, {
    Map<String, dynamic>? queryParameters,
  }) async {
    try {
      final response = await _dio.get<List<int>>(
        _resolvePath(path),
        queryParameters: queryParameters,
        options: Options(
          responseType: ResponseType.bytes,
          headers: {'Accept': '*/*'},
        ),
      );
      final bytes = response.data;
      if (bytes == null || bytes.isEmpty) {
        throw ApiException(message: 'استجابة فارغة من الخادم');
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
      throw _mapDioError(e);
    }
  }

  /// Builds an authenticated browser URL for file download fallbacks.
  Future<String> buildAuthenticatedDownloadUrl(
    String path, {
    Map<String, dynamic>? queryParameters,
  }) async {
    final token = await _authStorage.getToken();
    final params = queryParameters ?? const <String, dynamic>{};
    final query = <String, String>{
      for (final entry in params.entries)
        if (entry.value != null) entry.key: entry.value.toString(),
      if (token != null && token.isNotEmpty) 'access_token': token,
    };

    return Uri.parse('${UnifiedApiConfig.teacherBaseUrl}${_resolvePath(path)}')
        .replace(queryParameters: query.isEmpty ? null : query)
        .toString();
  }

  Future<void> postVoid(String path, {Map<String, dynamic>? body}) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        _resolvePath(path),
        data: body,
      );
      _ensureSuccess(response.data);
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  Future<String> deleteCommand(String path) async {
    try {
      final response =
          await _dio.delete<Map<String, dynamic>>(_resolvePath(path));
      final json = response.data;
      _ensureSuccess(json);

      final data = json?['data'];
      if (data is Map<String, dynamic>) {
        final dataMessage = data['message'] as String?;
        if (dataMessage != null && dataMessage.isNotEmpty) {
          return dataMessage;
        }
      }

      final envelopeMessage = json?['message'] as String?;
      if (envelopeMessage != null && envelopeMessage.isNotEmpty) {
        return envelopeMessage;
      }

      return 'تمت العملية بنجاح';
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  Future<T> put<T>(
    String path, {
    Map<String, dynamic>? body,
    required T Function(dynamic json) parseData,
  }) async {
    try {
      final response = await _dio.put<Map<String, dynamic>>(
        _resolvePath(path),
        data: body,
      );
      return _parseEnvelope(response.data, parseData);
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  Future<String> putCommand(String path, {Map<String, dynamic>? body}) async {
    try {
      final response = await _dio.put<Map<String, dynamic>>(
        _resolvePath(path),
        data: body,
      );
      final json = response.data;
      _ensureSuccess(json);

      final data = json?['data'];
      if (data is Map<String, dynamic>) {
        final dataMessage = data['message'] as String?;
        if (dataMessage != null && dataMessage.isNotEmpty) {
          return dataMessage;
        }
      }

      final envelopeMessage = json?['message'] as String?;
      if (envelopeMessage != null && envelopeMessage.isNotEmpty) {
        return envelopeMessage;
      }

      return 'تمت العملية بنجاح';
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  /// POST that returns a user-facing message from [data.message] or [message].
  Future<String> postCommand(String path, {Map<String, dynamic>? body}) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        _resolvePath(path),
        data: body,
      );
      final json = response.data;
      _ensureSuccess(json);

      final data = json?['data'];
      if (data is Map<String, dynamic>) {
        final dataMessage = data['message'] as String?;
        if (dataMessage != null && dataMessage.isNotEmpty) {
          return dataMessage;
        }
      }

      final envelopeMessage = json?['message'] as String?;
      if (envelopeMessage != null && envelopeMessage.isNotEmpty) {
        return envelopeMessage;
      }

      return 'تمت العملية بنجاح';
    } on DioException catch (e) {
      throw _mapDioError(e);
    }
  }

  void _ensureSuccess(Map<String, dynamic>? json) {
    if (json == null) {
      throw ApiException(message: 'استجابة فارغة من الخادم');
    }

    final envelope = GlobalResponse<dynamic>.fromJson(json);
    if (!envelope.success) {
      throw ApiException(
        message: envelope.message.isNotEmpty
            ? envelope.message
            : 'حدث خطأ أثناء تنفيذ الطلب',
        statusCode: envelope.statusCode,
      );
    }
  }

  T _parseEnvelope<T>(
    Map<String, dynamic>? json,
    T Function(dynamic json) parseData,
  ) {
    if (json == null) {
      throw ApiException(message: 'استجابة فارغة من الخادم');
    }

    final envelope = GlobalResponse<T>.fromJson(
      json,
      fromJsonT: parseData,
    );

    if (!envelope.success) {
      throw ApiException(
        message: envelope.message.isNotEmpty
            ? envelope.message
            : 'حدث خطأ أثناء تنفيذ الطلب',
        statusCode: envelope.statusCode,
      );
    }

    if (envelope.data == null) {
      throw ApiException(message: 'لا توجد بيانات في الاستجابة');
    }

    return envelope.data as T;
  }

  ApiException _mapDioError(DioException error) {
    final fromBody = _messageFromErrorBody(error.response?.data);
    if (fromBody != null) {
      return ApiException(
        message: fromBody,
        statusCode: error.response?.statusCode,
      );
    }

    switch (error.type) {
      case DioExceptionType.connectionTimeout:
      case DioExceptionType.receiveTimeout:
      case DioExceptionType.sendTimeout:
        return ApiException(message: 'انتهت مهلة الاتصال بالخادم');
      case DioExceptionType.connectionError:
        return ApiException(
          message: 'تعذر الاتصال بالخادم. تحقق من عنوان API والشبكة',
        );
      default:
        return ApiException(
          message: error.message ?? 'حدث خطأ في الاتصال',
          statusCode: error.response?.statusCode,
        );
    }
  }

  /// Extracts `message` from JSON maps, JSON strings, or byte bodies
  /// (file-download endpoints often return API errors as bytes).
  String? _messageFromErrorBody(dynamic data) {
    if (data == null) return null;

    if (data is Map) {
      final message = data['message']?.toString();
      if (message != null && message.isNotEmpty) return message;
      return null;
    }

    String? raw;
    if (data is String) {
      raw = data;
    } else if (data is List<int>) {
      try {
        raw = utf8.decode(data);
      } catch (_) {
        return null;
      }
    }

    if (raw == null || raw.trim().isEmpty) return null;

    try {
      final json = jsonDecode(raw);
      if (json is Map) {
        final message = json['message']?.toString();
        if (message != null && message.isNotEmpty) return message;
      }
    } catch (_) {
      // Not JSON — ignore.
    }
    return null;
  }
}
