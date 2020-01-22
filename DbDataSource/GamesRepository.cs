using Core.Components;
using Core.Exceptions;
using Core.Models;
using Core.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DbDataSource
{
    public class GamesRepository : IGamesRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Game> _games;
        private readonly ILogger _logger;

        public GamesRepository(IOptions<DbPersistenceSettings> settings, ILogger logger)
        {
            _logger = logger;
            _database = ConnectToDatabase(settings.Value.ConnectionString, settings.Value.DatabaseName);
            _logger.Debug($"Connected to database: {_database.DatabaseNamespace.DatabaseName}");
            _games = _database.GetCollection<Game>(settings.Value.CollectionName);
            _logger.Debug($"Loaded collection: {_games.CollectionNamespace.FullName}");
            _logger = logger;
        }

        public IReadOnlyDictionary<GameId, Game> Games =>
            GetAll().ToDictionary(
                    kv => kv.Id,
                    kv => kv);

        public void Add(Game game)
        {
            if (_games.Find(new ExpressionFilterDefinition<Game>(g => g.Id == game.Id)).ToEnumerable().Any())
            {
                throw new DuplicateKeyException("Game already added.");
            }

            _logger.Debug($"Adding new game: {game.Id} to database.");
            AddDocumentAsync(game).Wait();
            _logger.Debug($"Successfully added new game: {game.Id} to database.");
        }

        public Game NewGame()
        {
            Game game = new Game();
            Add(game);
            return game;
        }

        public void Update(Game game)
        {
            _logger.Debug($"Trying to update game: {game.Id} in database.");
            var timer = new Stopwatch();
            timer.Start();
            UpdateDocumentAsync(game).Wait();
            timer.Stop();
            _logger.Debug($"Successfully updated game: {game.Id} in database. Time: {timer.Elapsed.TotalMilliseconds}");
        }

        public bool TryFindOne(GameId id, out Game game)
        {
            game = _games.Find(g => g.Id == id).SingleOrDefault();
            return game != null;
        }

        public bool TryFindGameForDeal(DealId id, out Game game)
        {
            game = _games.Find(g => g.History.Any(d => d.Id == id)).SingleOrDefault();
            return game != null;
        }

        public void ClearAll()
        {
            _logger.Debug("Trying to clear all games from database.");
            DeleteAllAsync().Wait();
            _logger.Debug("Cleared all games from database.");
        }

        private IMongoDatabase ConnectToDatabase(string connectionString, string database)
        {
            var clientOptions = MongoClientSettings.FromConnectionString(connectionString);
            clientOptions.RetryWrites = false;
            var client = new MongoClient(clientOptions);
            return client.GetDatabase(database);
        }

        private async Task AddDocumentAsync(Game document)
        {
            await _games.InsertOneAsync(document);
        }

        private async Task UpdateDocumentAsync(Game document)
        {
            await _games.ReplaceOneAsync(g => g.Id.Value == document.Id.Value, document);
        }

        public IEnumerable<Game> GetAll()
        {
            return (_games.Find(FilterDefinition<Game>.Empty)).ToList();
        }

        private async Task DeleteAsync(Game document)
        {
            await _games.DeleteOneAsync(x => x.Id == document.Id);
        }

        private async Task DeleteAllAsync()
        {
            await _games.DeleteManyAsync(FilterDefinition<Game>.Empty);
        }
    }
}