using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HoPoSim.Data
{
	internal class Migration
	{
		public string Step { get; set; }
		public string ErrorMessage { get; set; }
		public Action<Context> Delegate { get; set; }

		public void Execute(Context context)
		{
			ExecuteSqlCommand(context);
			CallDelegate(context);
		}

		private void ExecuteSqlCommand(Context context)
		{
			if (!string.IsNullOrEmpty(Step))
				context.Database.ExecuteSqlCommand(Step);
		}

		private void CallDelegate(Context context)
		{
			Delegate?.Invoke(context);
		}
	}

	internal class MigrationHelper
	{
		public MigrationHelper()
		{
			Migrations = new Dictionary<int, IList>();

			MigrationVersion0();
			MigrationVersion2();
			MigrationVersion3();
			MigrationVersion4();
		}

		public Dictionary<int, IList> Migrations { get; set; }

		private void MigrationVersion0()
		{
			IList steps = new List<Migration>();
			//steps.Add("CREATE TABLE \"DbVersion\" (\"Id\" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , \"Version\" INTEGER)");
			Migrations.Add(0, steps);
			Migrations.Add(1, steps);
		}

		private void MigrationVersion2()
		{
			IList steps = new List<Migration>();
			var migration = new Migration();
			migration.Step =
				@"PRAGMA foreign_keys = 0;
				BEGIN TRANSACTION;

				CREATE TABLE IF NOT EXISTS BaumartParametrizations
				(   Id INTEGER NOT NULL CONSTRAINT PK_BaumartParametrizations PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, MinNoiseStrength REAL NOT NULL, MaxNoiseStrength REAL NOT NULL, MinNoiseSize REAL NOT NULL, MaxNoiseSize REAL NOT NULL, IncludeRoots INTEGER NOT NULL, MinRootFlareNumber INTEGER NOT NULL, MaxRootFlareNumber INTEGER NOT NULL, MinRootRadiusMultiplier REAL NOT NULL, MaxRootRadiusMultiplier REAL NOT NULL);

				CREATE TABLE IF NOT EXISTS Stapelqualitäten
				(   Id INTEGER NOT NULL CONSTRAINT Stapelqualitäten PRIMARY KEY AUTOINCREMENT, Level INTEGER NOT NULL, CrossTrunksProportion INTEGER NOT NULL, CrossTrunksMinimumAngle INTEGER NOT NULL, CrossTrunksMaximumAngle INTEGER NOT NULL, Bemerkungen TEXT);

				ALTER TABLE SimulationData ADD COLUMN Rindenbeschädigungen INTEGER NOT NULL default 0;
				ALTER TABLE SimulationData ADD COLUMN Krümmungsvarianten INTEGER NOT NULL default 0;
				ALTER TABLE SimulationData ADD COLUMN BaumartId REFERENCES BaumartParametrizations(Id) default 1;
				ALTER TABLE SimulationData ADD COLUMN StapelqualitätId REFERENCES Stapelqualitäten(Id) default 1;

				ALTER TABLE GeneratorData ADD COLUMN StammfußAnteil INTEGER NOT NULL default 0;
				ALTER TABLE GeneratorData ADD COLUMN StammfußMinHeight INTEGER NOT NULL default 0;
				ALTER TABLE GeneratorData ADD COLUMN StammfußMaxHeight INTEGER NOT NULL default 0;

				COMMIT;
				PRAGMA foreign_keys = 1;";
			migration.ErrorMessage = "SimulationData Migration kann nicht durchgeführt werden.";
			steps.Add(migration);
			Migrations.Add(2, steps);
		}

		private void MigrationVersion3()
		{
			IList steps = new List<Migration>();
			var migration = new Migration();
			migration.Step =
				@"PRAGMA foreign_keys = 0;

				DROP TABLE Stapelqualitäten;
				CREATE TABLE IF NOT EXISTS Stapelqualitäten
				(   Id INTEGER NOT NULL CONSTRAINT Stapelqualitäten PRIMARY KEY AUTOINCREMENT, Level INTEGER NOT NULL, CrossTrunksProportion INTEGER NOT NULL, CrossTrunksMinimumAngle INTEGER NOT NULL, CrossTrunksMaximumAngle INTEGER NOT NULL, Bemerkungen TEXT);


				DROP TABLE BaumartParametrizations;

				CREATE TABLE IF NOT EXISTS BaumartParametrizations
				(   Id INTEGER NOT NULL CONSTRAINT PK_BaumartParametrizations PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, MinNoiseStrength REAL NOT NULL, MaxNoiseStrength REAL NOT NULL, MinNoiseSize REAL NOT NULL, MaxNoiseSize REAL NOT NULL, 
					IncludeRoots INTEGER NOT NULL, MinRootFlareNumber INTEGER NOT NULL, MaxRootFlareNumber INTEGER NOT NULL, MinRootRadiusMultiplier REAL NOT NULL, MaxRootRadiusMultiplier REAL NOT NULL,
					IncludeBranches INTEGER NOT NULL, BranchStubTrunkProportion INTEGER NOT NULL, BranchStubMinLength INTEGER NOT NULL, BranchStubMaxLength INTEGER NOT NULL, BranchStubMinHeight REAL NOT NULL, BranchStubMaxHeight REAL NOT NULL, BranchStubAverageAngle REAL NOT NULL, BranchStubRadiusMultiplier REAL NOT NULL, BranchStubNumberPerMeter REAL NOT NULL);

				PRAGMA foreign_keys = 1;";
			migration.ErrorMessage = "Astigkeit Migration kann nicht durchgeführt werden.";
			steps.Add(migration);
			Migrations.Add(3, steps);
		}

		private void MigrationVersion4()
		{
			IList steps = new List<Migration>();
			var migration = new Migration();
			migration.Step =
				@"PRAGMA foreign_keys = 0;
				BEGIN TRANSACTION;

				ALTER TABLE SimulationData ADD COLUMN Poltertiefe REAL default NULL;

				COMMIT;
				PRAGMA foreign_keys = 1;";
			migration.ErrorMessage = "SimulationData Migration (Poltertiefe) kann nicht durchgeführt werden.";
			steps.Add(migration);
			Migrations.Add(4, steps);
		}
	}
}
