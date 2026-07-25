namespace AdminAPI.Services;

public static class ParentsFollowupPhotoValidation
{
    public const int AspectWidth = 4;
    public const int AspectHeight = 6;
    public const double AspectRatio = (double)AspectWidth / AspectHeight;
    public const double AspectTolerance = 0.05;
    public const string AspectErrorMessage = "يجب أن تكون الصورة بمقاس 4×6 (العرض × الارتفاع)";

    public static bool HasValidAspectRatio(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return false;

        var ratio = (double)width / height;
        return Math.Abs(ratio - AspectRatio) / AspectRatio <= AspectTolerance;
    }

    public static bool TryGetImageDimensions(Stream stream, out int width, out int height)
    {
        width = 0;
        height = 0;

        if (!stream.CanSeek)
            return false;

        var startPosition = stream.Position;

        try
        {
            if (TryReadPngDimensions(stream, out width, out height))
                return true;

            stream.Position = startPosition;
            return TryReadJpegDimensions(stream, out width, out height);
        }
        finally
        {
            stream.Position = startPosition;
        }
    }

    private static bool TryReadPngDimensions(Stream stream, out int width, out int height)
    {
        width = 0;
        height = 0;

        Span<byte> header = stackalloc byte[24];
        if (stream.Read(header) < 24)
            return false;

        if (header[0] != 0x89 || header[1] != (byte)'P' || header[2] != (byte)'N' || header[3] != (byte)'G')
            return false;

        width = ReadBigEndianInt32(header[16], header[17], header[18], header[19]);
        height = ReadBigEndianInt32(header[20], header[21], header[22], header[23]);
        return width > 0 && height > 0;
    }

    private static bool TryReadJpegDimensions(Stream stream, out int width, out int height)
    {
        width = 0;
        height = 0;

        if (stream.ReadByte() != 0xFF || stream.ReadByte() != 0xD8)
            return false;

        while (stream.Position < stream.Length)
        {
            var markerPrefix = stream.ReadByte();
            if (markerPrefix < 0)
                return false;

            if (markerPrefix != 0xFF)
                continue;

            var marker = stream.ReadByte();
            if (marker < 0)
                return false;

            if (marker is 0xD8 or 0xD9 or >= 0xD0 and <= 0xD7)
                continue;

            var lengthHigh = stream.ReadByte();
            var lengthLow = stream.ReadByte();
            if (lengthHigh < 0 || lengthLow < 0)
                return false;

            var segmentLength = (lengthHigh << 8) + lengthLow;
            if (segmentLength < 2)
                return false;

            var contentLength = segmentLength - 2;
            if (marker is 0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or 0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF)
            {
                Span<byte> frameHeader = stackalloc byte[5];
                if (stream.Read(frameHeader) < 5)
                    return false;

                height = ReadBigEndianInt16(frameHeader[1], frameHeader[2]);
                width = ReadBigEndianInt16(frameHeader[3], frameHeader[4]);
                return width > 0 && height > 0;
            }

            stream.Seek(contentLength, SeekOrigin.Current);
        }

        return false;
    }

    private static int ReadBigEndianInt32(byte b0, byte b1, byte b2, byte b3) =>
        (b0 << 24) | (b1 << 16) | (b2 << 8) | b3;

    private static int ReadBigEndianInt16(byte high, byte low) =>
        (high << 8) | low;
}
