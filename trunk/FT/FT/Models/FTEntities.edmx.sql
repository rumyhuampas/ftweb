
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/22/2012 04:48:42
-- Generated from EDMX file: C:\Users\Javier\Documents\Visual Studio 2010\Projects\FT\FT\Models\FTEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[ft].[FK__championship_matches_championships]', 'F') IS NOT NULL
    ALTER TABLE [ft].[championship_matches] DROP CONSTRAINT [FK__championship_matches_championships];
GO
IF OBJECT_ID(N'[ft].[FK__championship_matches_matches]', 'F') IS NOT NULL
    ALTER TABLE [ft].[championship_matches] DROP CONSTRAINT [FK__championship_matches_matches];
GO
IF OBJECT_ID(N'[ft].[FK__championship_teams_championship]', 'F') IS NOT NULL
    ALTER TABLE [ft].[championship_teams] DROP CONSTRAINT [FK__championship_teams_championship];
GO
IF OBJECT_ID(N'[ft].[FK_team_player_players]', 'F') IS NOT NULL
    ALTER TABLE [ft].[team_player] DROP CONSTRAINT [FK_team_player_players];
GO
IF OBJECT_ID(N'[ft].[FK_team_player_teams]', 'F') IS NOT NULL
    ALTER TABLE [ft].[team_player] DROP CONSTRAINT [FK_team_player_teams];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[ft].[championship_matches]', 'U') IS NOT NULL
    DROP TABLE [ft].[championship_matches];
GO
IF OBJECT_ID(N'[ft].[championship_teams]', 'U') IS NOT NULL
    DROP TABLE [ft].[championship_teams];
GO
IF OBJECT_ID(N'[ft].[championships]', 'U') IS NOT NULL
    DROP TABLE [ft].[championships];
GO
IF OBJECT_ID(N'[ft].[match_results]', 'U') IS NOT NULL
    DROP TABLE [ft].[match_results];
GO
IF OBJECT_ID(N'[ft].[matches]', 'U') IS NOT NULL
    DROP TABLE [ft].[matches];
GO
IF OBJECT_ID(N'[ft].[players]', 'U') IS NOT NULL
    DROP TABLE [ft].[players];
GO
IF OBJECT_ID(N'[ft].[team_player]', 'U') IS NOT NULL
    DROP TABLE [ft].[team_player];
GO
IF OBJECT_ID(N'[ft].[teams]', 'U') IS NOT NULL
    DROP TABLE [ft].[teams];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'championship_matches'
CREATE TABLE [dbo].[championship_matches] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [championship_Id] int  NOT NULL,
    [match_Id] int  NOT NULL
);
GO

-- Creating table 'championship_teams'
CREATE TABLE [dbo].[championship_teams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [championship_Id] int  NOT NULL,
    [team_Id] int  NOT NULL
);
GO

-- Creating table 'championships'
CREATE TABLE [dbo].[championships] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] longtext  NOT NULL,
    [Type] longtext  NOT NULL
);
GO

-- Creating table 'match_results'
CREATE TABLE [dbo].[match_results] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [match_Id] int  NOT NULL,
    [Set] int  NOT NULL,
    [team_a_games] int  NOT NULL,
    [team_b_games] int  NOT NULL
);
GO

-- Creating table 'matches'
CREATE TABLE [dbo].[matches] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [team_b_Id] int  NOT NULL,
    [team_a_Id] int  NOT NULL
);
GO

-- Creating table 'players'
CREATE TABLE [dbo].[players] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] longtext  NOT NULL,
    [Image] longtext  NULL
);
GO

-- Creating table 'team_player'
CREATE TABLE [dbo].[team_player] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Team_Id] int  NOT NULL,
    [Player_Id] int  NOT NULL
);
GO

-- Creating table 'teams'
CREATE TABLE [dbo].[teams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] longtext  NOT NULL,
    [Logo] longtext  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'championship_matches'
ALTER TABLE [dbo].[championship_matches]
ADD CONSTRAINT [PK_championship_matches]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'championship_teams'
ALTER TABLE [dbo].[championship_teams]
ADD CONSTRAINT [PK_championship_teams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'championships'
ALTER TABLE [dbo].[championships]
ADD CONSTRAINT [PK_championships]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'match_results'
ALTER TABLE [dbo].[match_results]
ADD CONSTRAINT [PK_match_results]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'matches'
ALTER TABLE [dbo].[matches]
ADD CONSTRAINT [PK_matches]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'players'
ALTER TABLE [dbo].[players]
ADD CONSTRAINT [PK_players]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'team_player'
ALTER TABLE [dbo].[team_player]
ADD CONSTRAINT [PK_team_player]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'teams'
ALTER TABLE [dbo].[teams]
ADD CONSTRAINT [PK_teams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [championship_Id] in table 'championship_matches'
ALTER TABLE [dbo].[championship_matches]
ADD CONSTRAINT [FK__championship_matches_championships]
    FOREIGN KEY ([championship_Id])
    REFERENCES [dbo].[championships]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK__championship_matches_championships'
CREATE INDEX [IX_FK__championship_matches_championships]
ON [dbo].[championship_matches]
    ([championship_Id]);
GO

-- Creating foreign key on [match_Id] in table 'championship_matches'
ALTER TABLE [dbo].[championship_matches]
ADD CONSTRAINT [FK__championship_matches_matches]
    FOREIGN KEY ([match_Id])
    REFERENCES [dbo].[matches]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK__championship_matches_matches'
CREATE INDEX [IX_FK__championship_matches_matches]
ON [dbo].[championship_matches]
    ([match_Id]);
GO

-- Creating foreign key on [championship_Id] in table 'championship_teams'
ALTER TABLE [dbo].[championship_teams]
ADD CONSTRAINT [FK__championship_teams_championship]
    FOREIGN KEY ([championship_Id])
    REFERENCES [dbo].[championships]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK__championship_teams_championship'
CREATE INDEX [IX_FK__championship_teams_championship]
ON [dbo].[championship_teams]
    ([championship_Id]);
GO

-- Creating foreign key on [Player_Id] in table 'team_player'
ALTER TABLE [dbo].[team_player]
ADD CONSTRAINT [FK_team_player_players]
    FOREIGN KEY ([Player_Id])
    REFERENCES [dbo].[players]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_team_player_players'
CREATE INDEX [IX_FK_team_player_players]
ON [dbo].[team_player]
    ([Player_Id]);
GO

-- Creating foreign key on [Team_Id] in table 'team_player'
ALTER TABLE [dbo].[team_player]
ADD CONSTRAINT [FK_team_player_teams]
    FOREIGN KEY ([Team_Id])
    REFERENCES [dbo].[teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_team_player_teams'
CREATE INDEX [IX_FK_team_player_teams]
ON [dbo].[team_player]
    ([Team_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------