IF OBJECT_ID(N'dbo.CircleVisitRatings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CircleVisitRatings (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        TeacherId INT NOT NULL,
        QuranCircleId INT NOT NULL,
        VisitDate DATE NOT NULL,
        VisitTime TIME NOT NULL,
        VisitNumberInMonth INT NOT NULL,
        CreatedBy INT NOT NULL,
        CreatedAt DATETIME NOT NULL,
        CONSTRAINT FK_CircleVisitRatings_Teacher
            FOREIGN KEY (TeacherId) REFERENCES dbo.Teacher(Id),
        CONSTRAINT FK_CircleVisitRatings_QuranCircle
            FOREIGN KEY (QuranCircleId) REFERENCES dbo.QuranCircle(Id)
    );
    CREATE INDEX IX_CircleVisitRatings_TeacherId_VisitDate
        ON dbo.CircleVisitRatings (TeacherId, VisitDate);
    CREATE INDEX IX_CircleVisitRatings_CreatedBy
        ON dbo.CircleVisitRatings (CreatedBy);
END
GO

IF OBJECT_ID(N'dbo.CircleVisitRatingItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CircleVisitRatingItems (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CircleVisitRatingId INT NOT NULL,
        Sequence INT NOT NULL,
        Criterion NVARCHAR(200) NOT NULL,
        Rating NVARCHAR(50) NOT NULL,
        Notes NVARCHAR(1000) NULL,
        CONSTRAINT FK_CircleVisitRatingItems_CircleVisitRatings
            FOREIGN KEY (CircleVisitRatingId)
            REFERENCES dbo.CircleVisitRatings(Id) ON DELETE CASCADE
    );
END
GO
