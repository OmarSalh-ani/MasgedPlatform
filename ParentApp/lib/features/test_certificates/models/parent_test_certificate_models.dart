class ParentTestCertificateListItem {
  const ParentTestCertificateListItem({
    required this.testId,
    required this.studentId,
    required this.studentName,
    required this.testDate,
    required this.grade,
    required this.totalScore,
    required this.testFrom,
    required this.testTo,
  });

  final int testId;
  final int studentId;
  final String studentName;
  final String testDate;
  final String grade;
  final String totalScore;
  final String testFrom;
  final String testTo;

  factory ParentTestCertificateListItem.fromJson(Map<String, dynamic> json) {
    return ParentTestCertificateListItem(
      testId: json['testId'] as int,
      studentId: json['studentId'] as int,
      studentName: (json['studentName'] ?? '').toString(),
      testDate: (json['testDate'] ?? '').toString(),
      grade: (json['grade'] ?? '').toString(),
      totalScore: (json['totalScore'] ?? '').toString(),
      testFrom: (json['testFrom'] ?? '').toString(),
      testTo: (json['testTo'] ?? '').toString(),
    );
  }
}
