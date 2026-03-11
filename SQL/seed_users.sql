-- Seed script for SocialMotive.Users table
-- Generated from contacts.json
-- Running on SocialMotive database
-- Created: March 11, 2026

SET IDENTITY_INSERT [dbo].[Users] ON;

INSERT INTO [dbo].[Users] 
    (UserId, ExternalSubjectId, Email, FirstName, LastName, Phone, DisplayName, Bio, IsActive, CreatedUtc, UpdatedUtc)
VALUES
    (2, 'sso|aria-evers-lam|' + LOWER(REPLACE('Aria Evers-Lam', ' ', '')), 'aria.evers-lam@socialmotive.local', 'Aria', 'Evers-Lam', '+31 6 55530737', 'Aria', NULL, 1, GETUTC(), GETUTC()),
    (3, 'sso|arno-de-bruijn|' + LOWER(REPLACE('Arno de Bruijn', ' ', '')), 'arno.debruijn@socialmotive.local', 'Arno', 'de Bruijn', '+31 6 54776142', 'Arno', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (5, 'sso|herman-waaijenberg|' + LOWER(REPLACE('Herman Waaijenberg', ' ', '')), 'herman.waaijenberg@socialmotive.local', 'Herman', 'Waaijenberg', '+31 6 12910932', 'Herman', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (6, 'sso|hilda-lubben|' + LOWER(REPLACE('Hilda Lubben', ' ', '')), 'hilda.lubben@socialmotive.local', 'Hilda', 'Lubben', '+31 6 53612916', 'Hilda', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (7, 'sso|ingrid|' + LOWER(REPLACE('Ingrid', ' ', '')), 'ingrid@socialmotive.local', 'Ingrid', '', '+31 6 51976666', 'Ingrid', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (8, 'sso|martha|' + LOWER(REPLACE('Martha', ' ', '')), 'martha@socialmotive.local', 'Martha', '', '+31 6 14343931', 'Martha', '🍀BBB🍀', 1, GETUTC(), GETUTC()),
    (9, 'sso|monique-peters|' + LOWER(REPLACE('Monique Peters', ' ', '')), 'monique.peters@socialmotive.local', 'Monique', 'Peters', '+31 6 23474027', 'Monique Peters', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (10, 'sso|paula|' + LOWER(REPLACE('Paula', ' ', '')), 'paula@socialmotive.local', 'Paula', '', '+31 6 54637473', 'Paula', 'Beschikbaar', 1, GETUTC(), GETUTC()),
    (11, 'sso|ria|' + LOWER(REPLACE('Ria', ' ', '')), 'ria@socialmotive.local', 'Ria', '', '+31 6 81315786', 'Ria', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (12, 'sso|sandra|' + LOWER(REPLACE('Sandra', ' ', '')), 'sandra@socialmotive.local', 'Sandra', '', '+31 6 50834998', 'Sandra', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (13, 'sso|silvia-beentjes|' + LOWER(REPLACE('silvia beentjes', ' ', '')), 'silvia.beentjes@socialmotive.local', 'Silvia', 'Beentjes', '+31 6 23546500', 'Silvia Beentjes', 'Hey there! I am using WhatsApp.', 1, GETUTC(), GETUTC()),
    (14, 'sso|tom-van-der-wind|' + LOWER(REPLACE('Tom Van der Wind', ' ', '')), 'tom.vanderwind@socialmotive.local', 'Tom', 'Van der Wind', '+31 6 21495345', 'Tom Van der Wind', NULL, 1, GETUTC(), GETUTC()),
    (15, 'sso|anton-de-vries|' + LOWER(REPLACE('Anton De Vries', ' ', '')), 'anton.devries@socialmotive.local', 'Anton', 'De Vries', NULL, 'Anton De Vries', NULL, 1, GETUTC(), GETUTC()),
    (16, 'sso|jack-kroon|' + LOWER(REPLACE('Jack Kroon', ' ', '')), 'jack.kroon@socialmotive.local', 'Jack', 'Kroon', NULL, 'Jack Kroon', NULL, 1, GETUTC(), GETUTC()),
    (17, 'sso|klaas-feenstra|' + LOWER(REPLACE('Klaas Feenstra SO', ' ', '')), 'klaas.feenstra@socialmotive.local', 'Klaas', 'Feenstra', NULL, 'Klaas Feenstra', 'Altijd in voor een goed gesprek 😎', 1, GETUTC(), GETUTC()),
    (18, 'sso|peter-fijvandraat|' + LOWER(REPLACE('Peter Fijvandraat SO', ' ', '')), 'peter.fijvandraat@socialmotive.local', 'Peter', 'Fijvandraat', NULL, 'Peter Fijvandraat', NULL, 1, GETUTC(), GETUTC()),
    (19, 'sso|rob-so|' + LOWER(REPLACE('Rob So', ' ', '')), 'rob.so@socialmotive.local', 'Rob', 'So', NULL, 'Rob So', NULL, 1, GETUTC(), GETUTC()),
    (20, 'sso|astrid|' + LOWER(REPLACE('Astrid', ' ', '')), 'astrid@socialmotive.local', 'Astrid', '', NULL, 'Astrid', NULL, 0, GETUTC(), GETUTC()),
    (21, 'sso|elly-strik|' + LOWER(REPLACE('Elly Strik', ' ', '')), 'elly.strik@socialmotive.local', 'Elly', 'Strik', '+31 6 12029642', 'Elly Strik', 'Nieuwe telefoon, whatsappgeschiedenis verdwenen.', 0, GETUTC(), GETUTC()),
    (22, 'sso|fred|' + LOWER(REPLACE('Fred', ' ', '')), 'fred@socialmotive.local', 'Fred', '', '+31 6 22536527', 'Fred', NULL, 0, GETUTC(), GETUTC()),
    (23, 'sso|lydia-van-den-berg|' + LOWER(REPLACE('Lydia van den Berg', ' ', '')), 'lydia.vandenberg@socialmotive.local', 'Lydia', 'van den Berg', '+31 6 18465544', 'Lydia van den Berg', 'Hey there! I am using WhatsApp.', 0, GETUTC(), GETUTC()),
    (24, 'sso|margriet-de-greef|' + LOWER(REPLACE('Margriet de Greef', ' ', '')), 'margriet.degreef@socialmotive.local', 'Margriet', 'de Greef', '+31 6 12327289', 'Margriet de Greef', 'Goeiedag 🌍', 0, GETUTC(), GETUTC()),
    (25, 'sso|marian|' + LOWER(REPLACE('Marian', ' ', '')), 'marian@socialmotive.local', 'Marian', '', '+31 6 42396291', 'Marian', NULL, 0, GETUTC(), GETUTC()),
    (26, 'sso|monique-harinck|' + LOWER(REPLACE('Monique Harinck', ' ', '')), 'monique.harinck@socialmotive.local', 'Monique', 'Harinck', '+31 6 21447368', 'Monique Harinck', NULL, 0, GETUTC(), GETUTC()),
    (27, 'sso|muisje|' + LOWER(REPLACE('Muisje', ' ', '')), 'muisje@socialmotive.local', 'Muisje', '', '+31 6 54742636', 'Muisje', NULL, 0, GETUTC(), GETUTC()),
    (28, 'sso|rijanne|' + LOWER(REPLACE('Rijanne', ' ', '')), 'rijanne@socialmotive.local', 'Rijanne', '', '+31 6 43926254', 'Rijanne', NULL, 0, GETUTC(), GETUTC()),
    (29, 'sso|tessa|' + LOWER(REPLACE('tessa', ' ', '')), 'tessa@socialmotive.local', 'Tessa', '', '+31 6 11191908', 'Tessa', 'Hey there! I am using WhatsApp.', 0, GETUTC(), GETUTC()),
    (30, 'sso|yve|' + LOWER(REPLACE('YvE', ' ', '')), 'yve@socialmotive.local', 'YvE', '', '+31 6 48560173', 'YvE', NULL, 0, GETUTC(), GETUTC());

SET IDENTITY_INSERT [dbo].[Users] OFF;

-- Verify inserts
SELECT COUNT(*) AS TotalUsersSeeded FROM [dbo].[Users];
SELECT TOP 5 UserId, DisplayName, Email, Phone FROM [dbo].[Users] ORDER BY UserId;

PRINT 'Seed complete: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' users inserted.';
