namespace ChatApp.Infrastructure.Persistence;

/// <summary>
/// Contains database constraint and index names referenced by application code
/// </summary>
public static class DatabaseConstraintNames
{
    public const string UniqueChannelNamePerWorkspace =
        "IX_Channels_WorkspaceId_Name";
}