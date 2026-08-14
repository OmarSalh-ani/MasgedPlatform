import '../models/memorizing_archive_item.dart';

const kArchiveTypeMemorizing = 'حفظ';
const kArchiveTypeRevision = 'مراجعة';
const kArchiveUnitJozz = 'جزء';
const kArchiveUnitHezb = 'حزب';

extension MemorizingArchiveItemDisplay on MemorizingArchiveItem {
  bool get isJuzHizbReview =>
      theType == kArchiveTypeRevision &&
      (surahName == kArchiveUnitJozz || surahName == kArchiveUnitHezb);

  bool get isMemorizing => theType == kArchiveTypeMemorizing;

  bool get isRevision => theType == kArchiveTypeRevision;

  String get unitOrSurahLabel {
    if (isJuzHizbReview) return surahName;
    if (surahName.trim().isNotEmpty) return surahName;
    return '—';
  }

  String get unitNumberLabel {
    if (isJuzHizbReview) return testFrom;
    return '';
  }
}
