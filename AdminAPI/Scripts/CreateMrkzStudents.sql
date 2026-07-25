IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'MrkzStudents')
BEGIN
    CREATE TABLE MrkzStudents (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        StudentName NVARCHAR(300) NOT NULL,
        FatherName NVARCHAR(300) NOT NULL,
        Age INT NOT NULL,
        StudentPhone NVARCHAR(50) NULL,
        FatherPhone NVARCHAR(50) NOT NULL,
        FatherPhone2 NVARCHAR(50) NULL,
        StudentGender NVARCHAR(5) NOT NULL,
        CreatedAt DATETIME NULL CONSTRAINT DF_MrkzStudents_CreatedAt DEFAULT (GETDATE()),
        QuranCircleId INT NULL,
        Birthdate DATETIME NULL,
        IsGirl INT NULL,
        FullName NVARCHAR(500) NULL,
        WomanActivityType INT NULL,
        LearnCertificate NVARCHAR(500) NULL,
        ThePassword NVARCHAR(250) NULL,
        IsSpecial BIT NOT NULL CONSTRAINT DF_MrkzStudents_IsSpecial DEFAULT (0),
        IsElite BIT NOT NULL DEFAULT (0),
        PlanLevelId INT NULL,
        CONSTRAINT FK_MrkzStudents_QuranCircle FOREIGN KEY (QuranCircleId) REFERENCES QuranCircle (Id),
        CONSTRAINT FK_MrkzStudents_PlanLevel FOREIGN KEY (PlanLevelId) REFERENCES PlanLevel (Id),
        CONSTRAINT FK_MrkzStudents_WomanActivity FOREIGN KEY (WomanActivityType) REFERENCES WomanActivity (Id)
    );
END
