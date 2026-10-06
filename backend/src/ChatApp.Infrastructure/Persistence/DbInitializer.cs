using ChatApp.Domain.Entities;
using ChatApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        await dbContext.Database.MigrateAsync();

        var seedPasswords = GetSeedPasswords(configuration);

        var now = DateTime.UtcNow;

        var alice = await GetOrCreateUserAsync(
            dbContext,
            username: "alice",
            email: "alice@example.com",
            password: seedPasswords.Alice,
            createdAt: now.AddDays(-30));

        var bob = await GetOrCreateUserAsync(
            dbContext,
            username: "bob",
            email: "bob@example.com",
            password: seedPasswords.Bob,
            createdAt: now.AddDays(-25));

        var charlie = await GetOrCreateUserAsync(
            dbContext,
            username: "charlie",
            email: "charlie@example.com",
            password: seedPasswords.Charlie,
            createdAt: now.AddDays(-20));

        var demo = await GetOrCreateUserAsync(
            dbContext,
            username: "demo",
            email: "demo@example.com",
            password: seedPasswords.Demo,
            createdAt: now.AddDays(-10));


        var developmentWorkspace =
            await GetOrCreateWorkspaceAsync(
                dbContext,
                name: "Development",
                description: "Development team workspace",
                createdAt: now.AddDays(-18));

        var designWorkspace =
            await GetOrCreateWorkspaceAsync(
                dbContext,
                name: "Design Team",
                description: "Design and product collaboration workspace",
                createdAt: now.AddDays(-16));

        var testingWorkspace =
            await GetOrCreateWorkspaceAsync(
                dbContext,
                name: "Testing",
                description: "Testing, QA and bug tracking workspace",
                createdAt: now.AddDays(-14));


        await EnsureMembershipAsync(
            dbContext,
            developmentWorkspace.Id,
            alice.Id,
            WorkspaceRole.Owner,
            now.AddDays(-18));

        await EnsureMembershipAsync(
            dbContext,
            developmentWorkspace.Id,
            bob.Id,
            WorkspaceRole.Admin,
            now.AddDays(-17));

        await EnsureMembershipAsync(
            dbContext,
            developmentWorkspace.Id,
            charlie.Id,
            WorkspaceRole.Member,
            now.AddDays(-16));

        await EnsureMembershipAsync(
            dbContext,
            developmentWorkspace.Id,
            demo.Id,
            WorkspaceRole.Member,
            now.AddDays(-10));

        await EnsureMembershipAsync(
            dbContext,
            designWorkspace.Id,
            alice.Id,
            WorkspaceRole.Member,
            now.AddDays(-16));

        await EnsureMembershipAsync(
            dbContext,
            designWorkspace.Id,
            bob.Id,
            WorkspaceRole.Owner,
            now.AddDays(-16));

        await EnsureMembershipAsync(
            dbContext,
            designWorkspace.Id,
            charlie.Id,
            WorkspaceRole.Admin,
            now.AddDays(-15));

        await EnsureMembershipAsync(
            dbContext,
            designWorkspace.Id,
            demo.Id,
            WorkspaceRole.Member,
            now.AddDays(-10));

        await EnsureMembershipAsync(
            dbContext,
            testingWorkspace.Id,
            alice.Id,
            WorkspaceRole.Admin,
            now.AddDays(-14));

        await EnsureMembershipAsync(
            dbContext,
            testingWorkspace.Id,
            bob.Id,
            WorkspaceRole.Member,
            now.AddDays(-13));

        await EnsureMembershipAsync(
            dbContext,
            testingWorkspace.Id,
            charlie.Id,
            WorkspaceRole.Owner,
            now.AddDays(-14));

        await EnsureMembershipAsync(
            dbContext,
            testingWorkspace.Id,
            demo.Id,
            WorkspaceRole.Member,
            now.AddDays(-10));


        var developmentGeneral =
            await GetOrCreateChannelAsync(
                dbContext,
                developmentWorkspace.Id,
                "general",
                ChannelType.Text,
                now.AddDays(-18));

        var developmentDevelopment =
            await GetOrCreateChannelAsync(
                dbContext,
                developmentWorkspace.Id,
                "development",
                ChannelType.Text,
                now.AddDays(-17));

        var developmentTesting =
            await GetOrCreateChannelAsync(
                dbContext,
                developmentWorkspace.Id,
                "testing",
                ChannelType.Text,
                now.AddDays(-16));

        var designGeneral =
            await GetOrCreateChannelAsync(
                dbContext,
                designWorkspace.Id,
                "general",
                ChannelType.Text,
                now.AddDays(-16));

        var designIdeas =
            await GetOrCreateChannelAsync(
                dbContext,
                designWorkspace.Id,
                "ideas",
                ChannelType.Text,
                now.AddDays(-15));

        var designFeedback =
            await GetOrCreateChannelAsync(
                dbContext,
                designWorkspace.Id,
                "feedback",
                ChannelType.Text,
                now.AddDays(-14));

        var testingGeneral =
            await GetOrCreateChannelAsync(
                dbContext,
                testingWorkspace.Id,
                "general",
                ChannelType.Text,
                now.AddDays(-14));

        var testingBugs =
            await GetOrCreateChannelAsync(
                dbContext,
                testingWorkspace.Id,
                "bugs",
                ChannelType.Text,
                now.AddDays(-13));

        var testingQa =
            await GetOrCreateChannelAsync(
                dbContext,
                testingWorkspace.Id,
                "qa",
                ChannelType.Text,
                now.AddDays(-12));


        await EnsureMessageAsync(
            dbContext,
            developmentGeneral.Id,
            alice.Id,
            "Welcome to the development workspace!",
            now.AddDays(-10));

        await EnsureMessageAsync(
            dbContext,
            developmentGeneral.Id,
            bob.Id,
            "Glad to be here. Let's get started.",
            now.AddDays(-9));

        await EnsureMessageAsync(
            dbContext,
            developmentGeneral.Id,
            demo.Id,
            "Hi everyone! I'm joining the project as a demo user.",
            now.AddDays(-8));

        await EnsureMessageAsync(
            dbContext,
            developmentDevelopment.Id,
            alice.Id,
            "The new API implementation is ready.",
            now.AddDays(-8));

        await EnsureMessageAsync(
            dbContext,
            developmentDevelopment.Id,
            charlie.Id,
            "I'll take a look at it today.",
            now.AddDays(-7));

        await EnsureMessageAsync(
            dbContext,
            designGeneral.Id,
            bob.Id,
            "Welcome to the design workspace.",
            now.AddDays(-8));

        await EnsureMessageAsync(
            dbContext,
            designGeneral.Id,
            demo.Id,
            "Looking forward to checking out the design process.",
            now.AddDays(-7));

        await EnsureMessageAsync(
            dbContext,
            designIdeas.Id,
            charlie.Id,
            "I have a few ideas for the new interface.",
            now.AddDays(-6));

        await EnsureMessageAsync(
            dbContext,
            designFeedback.Id,
            alice.Id,
            "The latest mockups look great.",
            now.AddDays(-5));

        await EnsureMessageAsync(
            dbContext,
            testingGeneral.Id,
            charlie.Id,
            "Testing workspace is ready.",
            now.AddDays(-6));

        await EnsureMessageAsync(
            dbContext,
            testingGeneral.Id,
            demo.Id,
            "I'll help test the application from a regular member account.",
            now.AddDays(-5));

        await EnsureMessageAsync(
            dbContext,
            testingBugs.Id,
            alice.Id,
            "Found a problem with message pagination.",
            now.AddDays(-4));

        await EnsureMessageAsync(
            dbContext,
            testingBugs.Id,
            bob.Id,
            "I'll investigate the pagination issue.",
            now.AddDays(-3));

        await dbContext.SaveChangesAsync();
    }

    private static async Task<User> GetOrCreateUserAsync(
        AppDbContext dbContext,
        string username,
        string email,
        string password,
        DateTime createdAt)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x =>
                x.Username == username ||
                x.Email == email);

        if (user is not null)
        {
            return user;
        }

        user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            CreatedAt = createdAt,
        };

        await dbContext.Users.AddAsync(user);

        return user;
    }

    private static async Task<Workspace> GetOrCreateWorkspaceAsync(
        AppDbContext dbContext,
        string name,
        string description,
        DateTime createdAt)
    {
        var workspace = await dbContext.Workspaces
            .FirstOrDefaultAsync(x => x.Name == name);

        if (workspace is not null)
        {
            return workspace;
        }

        workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = createdAt,
        };

        await dbContext.Workspaces.AddAsync(workspace);

        return workspace;
    }

    private static async Task EnsureMembershipAsync(
        AppDbContext dbContext,
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role,
        DateTime joinedAt)
    {
        var membershipExists =
            await dbContext.WorkspaceMembers.AnyAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.UserId == userId);

        if (membershipExists)
        {
            return;
        }

        await dbContext.WorkspaceMembers.AddAsync(
            new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                UserId = userId,
                Role = role,
                JoinedAt = joinedAt,
            });
    }

    private static async Task<Channel> GetOrCreateChannelAsync(
        AppDbContext dbContext,
        Guid workspaceId,
        string name,
        ChannelType type,
        DateTime createdAt)
    {
        var channel = await dbContext.Channels
            .FirstOrDefaultAsync(x =>
                x.WorkspaceId == workspaceId &&
                x.Name == name);

        if (channel is not null)
        {
            return channel;
        }

        channel = new Channel
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            Name = name,
            Type = type,
            CreatedAt = createdAt,
        };

        await dbContext.Channels.AddAsync(channel);

        return channel;
    }

    private static async Task EnsureMessageAsync(
        AppDbContext dbContext,
        Guid channelId,
        Guid userId,
        string content,
        DateTime createdAt)
    {
        var messageExists =
            await dbContext.Messages.AnyAsync(x =>
                x.ChannelId == channelId &&
                x.UserId == userId &&
                x.Content == content);

        if (messageExists)
        {
            return;
        }

        await dbContext.Messages.AddAsync(
            new Message
            {
                Id = Guid.NewGuid(),
                ChannelId = channelId,
                UserId = userId,
                Content = content,
                CreatedAt = createdAt,
            });
    }

    private static SeedPasswords GetSeedPasswords(
        IConfiguration configuration)
    {
        return new SeedPasswords(
            GetRequiredPassword(
                configuration,
                "SEED_ALICE_PASSWORD"),

            GetRequiredPassword(
                configuration,
                "SEED_BOB_PASSWORD"),

            GetRequiredPassword(
                configuration,
                "SEED_CHARLIE_PASSWORD"),

            GetRequiredPassword(
                configuration,
                "SEED_DEMO_PASSWORD"));
    }

    private static string GetRequiredPassword(
        IConfiguration configuration,
        string key)
    {
        var password = configuration[key];

        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Required seed password '{key}' was not configured.");
        }

        return password;
    }

    private sealed record SeedPasswords(
        string Alice,
        string Bob,
        string Charlie,
        string Demo);
}