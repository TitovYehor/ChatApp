namespace ChatApp.Infrastructure.Persistence;

public static class DatabaseConstraintNames
{
    public const string UniqueChannelNamePerWorkspace =
        "IX_Channels_WorkspaceId_Name";
}