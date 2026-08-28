import 'package:flutter_riverpod/flutter_riverpod.dart';



class ForegroundPushMessage {

  const ForegroundPushMessage({

    required this.title,

    required this.body,

    this.isMeeting = false,

    this.meetingId,

    this.isChat = false,

    this.teacherId,

    this.studentId,

    this.teacherName,

    this.studentName,

    this.parentPhone,

    this.isTestCertificate = false,

    this.testId,

    this.certificateStudentId,

  });



  final String title;

  final String body;

  final bool isMeeting;

  final int? meetingId;

  final bool isChat;

  final int? teacherId;

  final int? studentId;

  final String? teacherName;

  final String? studentName;

  final String? parentPhone;

  final bool isTestCertificate;

  final int? testId;

  final int? certificateStudentId;

}



final foregroundPushMessageProvider =

    StateProvider<ForegroundPushMessage?>((ref) => null);

