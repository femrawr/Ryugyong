namespace Main.Source.Functions.Persistence
{
    public enum AutoRunStatus
    {
        FailedOpenAutoRunKey,
        FailedFindAutoRunValue,
        FailedOpenAllowedKey,
        AllowedValueMissing,
        NotAllowedToStart,
        AllowedToStart,
    }

    public enum WinLogonStatus
    {
        FailedOpenWinLogonKey,
        FailedFindWinLogonValue,
        OnlyHasDefaultValue,
        DoesNotContainPath,
        AllowedToStart,
    }
}
