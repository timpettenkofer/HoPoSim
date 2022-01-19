using HoPoSim.Data.Domain;
using HoPoSim.Data.Model;
using HoPoSim.Framework.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HoPoSim.Data
{

	public class Context : DbContext
	{
		public DbSet<GeneratorData> GeneratorData { get; set; }
		public DbSet<SimulationData> SimulationData { get; set; }
		public DbSet<BaumartParametrization> BaumartParametrizations { get; set; }
		public DbSet<Stapelqualität> Stapelqualitäten { get; set; }
		public DbSet<SimulationConfiguration> SimulationConfigurations { get; set; }
		public DbSet<SimulationResults> SimulationResults { get; set; }

		public DbSet<Distribution> Distribution { get; set; }
		public DbSet<Durchmesser> Durchmesser { get; set; }
		public DbSet<Abholzigkeit> Abholzigkeit { get; set; }
		public DbSet<Krümmung> Krümmung { get; set; }
		public DbSet<Ovalität> Ovalität { get; set; }

		// migrations
		public DbSet<DbVersion> DbVersion { get; set; }
		// update on any db breaking change
		private int _dbCurrentVersion = 4;

		private string _dbDir;
		private IInteractionService _interaction;

		public Context(string dbDir, IInteractionService interaction)
		{
			_dbDir = dbDir;
			_interaction = interaction;
			EnsureDatabaseInitialized();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder
				.Entity<SimulationData>()
				.Property(e => e.Stammliste)
				.HasConversion(
					v => v.AsString(),
					v => new Stammdaten(v));

			modelBuilder
				.Entity<GeneratorData>()
				.HasIndex(a => a.Name)
				.IsUnique();

			modelBuilder
				.Entity<SimulationData>()
				.HasIndex(a => a.Name)
				.IsUnique();

			modelBuilder
				.Entity<BaumartParametrization>()
				.HasIndex(a => a.Name)
				.IsUnique();

			modelBuilder
				.Entity<SimulationConfiguration>()
				.HasIndex(a => a.Name)
				.IsUnique();

			modelBuilder
				.Entity<SimulationData>()
				.Property(e => e.Stammliste)
				.HasConversion(
					v => v.AsString(),
					v => new Stammdaten(v));

			foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
			{
				relationship.DeleteBehavior = DeleteBehavior.Cascade;
			}

			base.OnModelCreating(modelBuilder);
		}

		// This method connects the context with the database
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string dataSource = GetDataSourceFilePath();
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = dataSource };
			var connectionString = connectionStringBuilder.ToString();
			var connection = new SqliteConnection(connectionString);
			SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
			optionsBuilder.UseSqlite(connection);
			optionsBuilder.UseLazyLoadingProxies();
			optionsBuilder.EnableSensitiveDataLogging(true);
		}



		private string GetDataSourceFilePath()
		{
			return Path.Combine(_dbDir, "hoposim.db");
		}

		private void EnsureDatabaseInitialized()
		{
			try
			{
				Database.EnsureCreated();
				if (!DoTableExists(DbVersion))
				{
					InitVersionTable();
					InitDebugDatabase();
				}
				else
					EnsureMigrated();

				SeedDatabase();

				NotifyUserOnDatabaseErrors = true;
			}
			catch (Exception e)
			{
				if (NotifyUserOnDatabaseErrors)
				{
					var message = $"Die Datenbankdatei '{GetDataSourceFilePath()}' konnte nicht geöffnet werden.\n" +
					"Bitte überprüfen Sie, ob die Datei existiert und ob sie zugreifbar ist.\n\n" +
					"Sie können jederzeit in den Anwendung-Einstellungen den Ordner ändern, in welchem die Datenbankdatei zu finden ist.\n\n" +
					e.Message;
					_interaction.RaiseNotificationAsync(message, "Datenbankverbindungsfehler");
					NotifyUserOnDatabaseErrors = false;
				}
			}
		}

		private void InitVersionTable()
		{
			if (!DbVersion.Any())
			{
				var version = new DbVersion() { Version = _dbCurrentVersion };
				DbVersion.Add(version);
				SaveChanges();
			}
		}

		public int ActualDatabaseVersion { get; private set; }

		private void EnsureMigrated()
		{
			ActualDatabaseVersion = -1;
			if (DoTableExists(DbVersion))
			{
				ActualDatabaseVersion = DbVersion.Max(x => x.Version);
			}

			var migrationHelper = new MigrationHelper();
			var hasErrors = false;
			while (!hasErrors && ActualDatabaseVersion < _dbCurrentVersion)
			{
				ActualDatabaseVersion++;
				foreach (Migration migration in migrationHelper.Migrations[ActualDatabaseVersion])
				{
					try
					{
						migration.Execute(this);
					}
					catch (Exception e)
					{
						if (NotifyUserOnMigrationErrors)
						{
							_interaction.RaiseNotificationAsync(migration.ErrorMessage, "Datenbankmigrationsfehler");
							NotifyUserOnMigrationErrors = false;
						}
						hasErrors = true;
						break;
					}
				}
				if (!hasErrors)
				{
					DbVersion.Add(new DbVersion() { Version = ActualDatabaseVersion });
					SaveChanges();
				}
			}
		}

		public static bool NotifyUserOnDatabaseErrors { get; set; } = true;
		public static bool NotifyUserOnMigrationErrors { get; set; } = true;

		bool DoTableExists<TEntity>(DbSet<TEntity> table) where TEntity : class
		{
			try
			{
				var any = table.Any();
				return any;
			}
			catch (Exception)
			{
				return false;
			}
		}

		[Conditional("DEBUG")]
		private void InitDebugDatabase()
		{
			Parameter<Durchmesser,int> durchmesser = null;
			Parameter<Abholzigkeit,int> abholzigkeit = null;
			Parameter<Krümmung,int> krümmung = null;
			Parameter<Ovalität,double> ovalität = null;

			if (!Durchmesser.Any())
			{
				var values = Enumerable.Range(1, 4)
					.Select(i => new Durchmesser { RangeId = i, MinValue = 100 * i, MaxValue = 100 * i + 100, Rindenstärke = 10 * i });
				durchmesser = new Parameter<Durchmesser,int>(values);
				SaveChanges();
			}

			if (!Abholzigkeit.Any())
			{
				var values = Enumerable.Range(1, 2)
					.Select(i => new Abholzigkeit { RangeId = i, MinValue = 10 * i, MaxValue = 10 * (i + 1) })
					.ToList();
				abholzigkeit = new Parameter<Abholzigkeit,int>(values);
				SaveChanges();
			}

			if (!Krümmung.Any())
			{
				var values = Enumerable.Range(1, 2)
					.Select(i => new Krümmung { RangeId = i, MinValue = 10 * i, MaxValue = 10 * (i + 1) })
					.ToList();
				krümmung = new Parameter<Krümmung,int>(values);
				SaveChanges();
			}

			if (!Ovalität.Any())
			{
				var values = Enumerable.Range(1, 2)
					.Select(i => new Ovalität { RangeId = i, MinValue = 0.45 * i, MaxValue = 0.5 * i })
					.ToList();
				ovalität = new Parameter<Ovalität,double>(values);
				SaveChanges();
			}

			if (!GeneratorData.Any())
			{
				var data = new GeneratorData(4,2,2,2)
				{
					Name = "Auto_Daten",
					Stammanzahl = 80,
					Länge = 5.0f,
					LängeVariation = 1.0f,
					Bemerkungen = "Initialization test data",
					Durchmesser = durchmesser,
					Abholzigkeit = abholzigkeit,
					Krümmung = krümmung,
					Ovalität = ovalität
				};

				data.Distribution.Total = 80;
				data.Distribution.Absolute = 80;

				var durchmesser_dist = data.Distribution.Children.ToList();
				durchmesser_dist.ForEach(d => { d.Total = 80; d.Percent = 25.0; d.Absolute = 20; });

				var abholzhigkeit_dist = durchmesser_dist.SelectMany(a => a.Children).ToList();
				abholzhigkeit_dist.ForEach(d => { d.Total = 20; d.Percent = 50.0;  d.Absolute = 10; });

				var krümmung_dist = abholzhigkeit_dist.SelectMany(a => a.Children).ToList();
				krümmung_dist.ForEach(d => { d.Total = 10; d.Percent = 50.0; d.Absolute = 5; });

				var ovalität_dist = krümmung_dist.SelectMany(a => a.Children).ToList();
				ovalität_dist.ForEach(d => { d.Total = 5; d.Percent = d.RangeId * d.RangeId * 20; d.Absolute = Convert.ToInt32((d.Percent * d.Total) / 100); });

				GeneratorData.Add(data);
				SaveChanges();
			}
		}

		private void SeedDatabase()
		{
			if (!BaumartParametrizations.Any())
			{
				BaumartParametrizations.Add(new BaumartParametrization() { Name = "Undefiniert", MinNoiseStrength = 1, MaxNoiseStrength = 1, MinNoiseSize = 1, MaxNoiseSize = 1, IncludeRoots = false, MinRootFlareNumber = 0, MaxRootFlareNumber = 0, MinRootRadiusMultiplier = 1, MaxRootRadiusMultiplier = 1, IncludeBranches = false });
				BaumartParametrizations.Add(new BaumartParametrization() { Name = "Fichte", MinNoiseStrength = 0.6f, MaxNoiseStrength = 0.9f, MinNoiseSize = 1f, MaxNoiseSize = 1f, IncludeRoots = true, MinRootFlareNumber = 1, MaxRootFlareNumber = 4, MinRootRadiusMultiplier = 1.075f, MaxRootRadiusMultiplier = 1.4f, IncludeBranches = true, BranchStubTrunkProportion = 70, BranchStubMinLength=5, BranchStubMaxLength=30, BranchStubAverageAngle = 0.92f, BranchStubMinHeight = 0.05f, BranchStubMaxHeight = 0.95f, BranchStubNumberPerMeter = 6f, BranchStubRadiusMultiplier = 0.13f });
				BaumartParametrizations.Add(new BaumartParametrization() { Name = "Kiefer", MinNoiseStrength = 1, MaxNoiseStrength = 1, MinNoiseSize = 1, MaxNoiseSize = 1, IncludeRoots = false, MinRootFlareNumber = 0, MaxRootFlareNumber = 0, MinRootRadiusMultiplier = 1, MaxRootRadiusMultiplier = 1, IncludeBranches = false });
				BaumartParametrizations.Add(new BaumartParametrization() { Name = "Buche", MinNoiseStrength = 1, MaxNoiseStrength = 1, MinNoiseSize = 1, MaxNoiseSize = 1, IncludeRoots = false, MinRootFlareNumber = 0, MaxRootFlareNumber = 0, MinRootRadiusMultiplier = 1, MaxRootRadiusMultiplier = 1, IncludeBranches = false });

				SaveChanges();
			}

			if (!Stapelqualitäten.Any())
			{
				Stapelqualitäten.Add(new Stapelqualität() { Level = 0, CrossTrunksProportion = 0, CrossTrunksMinimumAngle = 0, CrossTrunksMaximumAngle = 0, Bemerkungen = "Beste Stapelqualität" });
				Stapelqualitäten.Add(new Stapelqualität() { Level = 1, CrossTrunksProportion = 15, CrossTrunksMinimumAngle = 0, CrossTrunksMaximumAngle = 5, Bemerkungen = "" });
				Stapelqualitäten.Add(new Stapelqualität() { Level = 2, CrossTrunksProportion = 15, CrossTrunksMinimumAngle = 5, CrossTrunksMaximumAngle = 10, Bemerkungen = "" });
				Stapelqualitäten.Add(new Stapelqualität() { Level = 3, CrossTrunksProportion = 20, CrossTrunksMinimumAngle = 5, CrossTrunksMaximumAngle = 15, Bemerkungen = "" });
				Stapelqualitäten.Add(new Stapelqualität() { Level = 4, CrossTrunksProportion = 30, CrossTrunksMinimumAngle = 5, CrossTrunksMaximumAngle = 15, Bemerkungen = "Schlechteste Stapelqualität" });

				SaveChanges();
			}
		}

	}
}

