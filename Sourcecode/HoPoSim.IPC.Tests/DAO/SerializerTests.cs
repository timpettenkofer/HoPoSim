using HoPoSim.IPC.DAO;
using NUnit.Framework;
using System.Collections.Generic;
using static HoPoSim.IPC.DAO.SimulationConfiguration;

namespace HoPoSim.IPC.Tests
{
	[TestFixture]
	public class SerializerTests : TestBase
	{
		private SimulationData GetTestSimulationData()
		{
			var polter = new PolterConfiguration()
			{
				MinimumPolterlänge = 3.0f,
				Polterbreite = 6.0f,
				Poltertiefe = null,
				Steigungswinkel = 46.0f,
				Seitenspiegelung = 10.5f,
				Zufallsspiegelung = false,
				Polterunterlage = false,
				CrossTrunksProportion = 25,
				CrossTrunksMinimumAngle = 10,
				CrossTrunksMaximumAngle = 28
			};

			var holz = new HolzConfiguration()
			{
				Rindenbeschädigungen = 10,
				Krümmungsvarianten = 60
			};

			var stämme = new List<Stamm>()
			{
				new Stamm(){ StammId ="1", Länge = 6.0f, D_Stirn_mR = 456, D_Mitte_mR = 400, D_Zopf_mR = 380, D_Stirn_oR = 450, D_Mitte_oR = 394, D_Zopf_oR = 374, Abholzigkeit = 12, Krümmung = 10, Rindenstärke = 6, Ovalität=0.9f},
				new Stamm(){ StammId ="2", Länge = 6.0f, D_Stirn_mR = 458, D_Mitte_mR = 402, D_Zopf_mR = 382, D_Stirn_oR = 452, D_Mitte_oR = 396, D_Zopf_oR = 376, Abholzigkeit = 10, Krümmung = 11, Rindenstärke = 5, Ovalität=0.8f, Stammfußhöhe=32},
			};

			var tree = new BaumartParametrization()
			{
				Name = "Buche",
				IncludeRoots = true,
				MinNoiseSize = 1.1f,
				MaxNoiseSize = 1.8f,
				MinNoiseStrength = 3.2f,
				MaxNoiseStrength = 5.1f,
				MinRootFlareNumber = 1,
				MaxRootFlareNumber = 4,
				MinRootRadiusMultiplier = 1.1f,
				MaxRootRadiusMultiplier = 1.5f,
				IncludeBranches = true,
				BranchStubTrunkProportion = 26,
				BranchStubMinLength = 5,
				BranchStubMaxLength = 33,
				BranchStubMinHeight = 0.23f,
				BranchStubMaxHeight = 0.81f,
				BranchStubAverageAngle = 0.9f,
				BranchStubRadiusMultiplier = 0.61f,
				BranchStubNumberPerMeter = 1.5f
			};

			var data = new SimulationData()
			{
				Id = 1,
				Name = "Test_Eingabe",
				Poltermaße = polter,
				Holz = holz,
				Baumart= tree,
				Stämme = stämme,
				WoodDensity = 500.0f,
				WoodFriction = 0.4f,
			};
			return data;
		}

		[Test]
		public void SimulationConfiguration_ToJSON_WithValidInput_ReturnsExpectedString()
		{
			var expected = GetReferenceDataFileAsText("RefFiles\\SimulationConfig_json.ref");
			var configuration = new SimulationConfiguration()
			{
				Id = 1,
				Name = "Test_Simulation",
				Comments = "unit test",
				Iterations = 10000,
				IterationStart = 0,
				Quality = 2,
				FotooptikQuality = 4,
				TimeOutPeriod = 10,
				Seed = 1234
			};

			var result = Serializer<SimulationConfiguration>.ToJSON(configuration);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void SimulationData_ToJSON_WithValidInput_ReturnsExpectedString()
		{
			var expected = GetReferenceDataFileAsText("RefFiles\\SimulationData_json.ref");
			var data = GetTestSimulationData();

			var result = Serializer<SimulationData>.ToJSON(data, true);

			Assert.AreEqual(expected, result);
		}


		[Test]
		public void SimulationConfiguration_ToJSON_WithValidData_ReturnsExpectedIndentedString()
		{
			var expected = "{\r\n  \"Id\": 1,\r\n  \"Name\": \"Test_Simulation\",\r\n  \"Comments\": \"unit test\",\r\n  \"Iterations\": 10000,\r\n  \"IterationStart\": 0,\r\n  \"TimeOutPeriod\": 10,\r\n  \"Seed\": 1234,\r\n  \"Quality\": 3,\r\n  \"FotooptikQuality\": 5\r\n}";
			var configuration = new SimulationConfiguration()
			{
				Id = 1,
				Name = "Test_Simulation",
				Comments = "unit test",
				Iterations = 10000,
				IterationStart = 0,
				TimeOutPeriod = 10,
				Quality = 3,
				FotooptikQuality = 5,
				Seed = 1234
			};

			var result = Serializer<SimulationConfiguration>.ToJSON(configuration, true);

			Assert.AreEqual(expected, result);
		}

		[Test]
		public void SimulationData_ToJSON_WithValidData_ReturnsExpectedIndentedString()
		{
			var expected = "{\r\n  \"Id\": 1,\r\n  \"Name\": \"Test_Eingabe\",\r\n  \"Poltermaße\": {\r\n    \"MinimumPolterlänge\": 3.0,\r\n    \"Polterbreite\": 6.0,\r\n    \"Poltertiefe\": null,\r\n    \"Polterunterlage\": false,\r\n    \"Steigungswinkel\": 46.0,\r\n    \"Seitenspiegelung\": 10.5,\r\n    \"Zufallsspiegelung\": false,\r\n    \"ActualPolterlänge\": 0.0,\r\n    \"Polterunterlagehöhe\": 0.0,\r\n    \"Polterunterlagestammanzahl\": 0,\r\n    \"CrossTrunksProportion\": 25,\r\n    \"CrossTrunksMinimumAngle\": 10,\r\n    \"CrossTrunksMaximumAngle\": 28\r\n  },\r\n  \"Holz\": {\r\n    \"Rindenbeschädigungen\": 10,\r\n    \"Krümmungsvarianten\": 60\r\n  },\r\n  \"Baumart\": {\r\n    \"Name\": \"Buche\",\r\n    \"MinNoiseStrength\": 3.2,\r\n    \"MaxNoiseStrength\": 5.1,\r\n    \"MinNoiseSize\": 1.1,\r\n    \"MaxNoiseSize\": 1.8,\r\n    \"IncludeRoots\": true,\r\n    \"MinRootFlareNumber\": 1,\r\n    \"MaxRootFlareNumber\": 4,\r\n    \"MinRootRadiusMultiplier\": 1.1,\r\n    \"MaxRootRadiusMultiplier\": 1.5,\r\n    \"IncludeBranches\": true,\r\n    \"BranchStubTrunkProportion\": 26,\r\n    \"BranchStubMinLength\": 5,\r\n    \"BranchStubMaxLength\": 33,\r\n    \"BranchStubMinHeight\": 0.23,\r\n    \"BranchStubMaxHeight\": 0.81,\r\n    \"BranchStubAverageAngle\": 0.9,\r\n    \"BranchStubRadiusMultiplier\": 0.61,\r\n    \"BranchStubNumberPerMeter\": 1.5\r\n  },\r\n  \"Stämme\": [\r\n    {\r\n      \"StammId\": \"1\",\r\n      \"Länge\": 6.0,\r\n      \"D_Stirn_mR\": 456,\r\n      \"D_Mitte_mR\": 400,\r\n      \"D_Zopf_mR\": 380,\r\n      \"D_Stirn_oR\": 450,\r\n      \"D_Mitte_oR\": 394,\r\n      \"D_Zopf_oR\": 374,\r\n      \"Abholzigkeit\": 12,\r\n      \"Krümmung\": 10,\r\n      \"Rindenstärke\": 6,\r\n      \"Ovalität\": 0.9,\r\n      \"Stammfußhöhe\": 0,\r\n      \"HasBranchStubs\": false\r\n    },\r\n    {\r\n      \"StammId\": \"2\",\r\n      \"Länge\": 6.0,\r\n      \"D_Stirn_mR\": 458,\r\n      \"D_Mitte_mR\": 402,\r\n      \"D_Zopf_mR\": 382,\r\n      \"D_Stirn_oR\": 452,\r\n      \"D_Mitte_oR\": 396,\r\n      \"D_Zopf_oR\": 376,\r\n      \"Abholzigkeit\": 10,\r\n      \"Krümmung\": 11,\r\n      \"Rindenstärke\": 5,\r\n      \"Ovalität\": 0.8,\r\n      \"Stammfußhöhe\": 32,\r\n      \"HasBranchStubs\": false\r\n    }\r\n  ],\r\n  \"WoodDensity\": 500.0,\r\n  \"WoodFriction\": 0.4\r\n}";
			var data = GetTestSimulationData();

			var result = Serializer<SimulationData>.ToJSON(data, true);

			Assert.AreEqual(expected, result);
		}


		[Test]
		public void Validate_WithValidConfig_ReturnsTrue()
		{
			var data = GetReferenceDataFileAsText("RefFiles\\SimulationConfig_json.ref");

			var result = Serializer<SimulationConfiguration>.Validate(data);

			Assert.IsTrue(result);
		}

		[Test]
		public void Validate_WithMissingRequiredIterationsProperty_ReturnsFalse()
		{
			var configWithMissingIterationsProperty = "{\r\n  \"Id\": 1,\r\n  \"Name\": \"Test_Simulation\",\r\n  \"Comments\": \"unit test\",\r\n    \"IterationStart\": 0,\r\n    \"TimeOutPeriod\": 10,\r\n    \"Seed\": 1234\r\n}";

			var result = Serializer<SimulationConfiguration>.Validate(configWithMissingIterationsProperty);

			Assert.IsFalse(result);
		}


		[Test]
		public void ValidateWithErrorMessages_WithMissingRequiredIterationsProperty_ReturnsExpectedError()
		{
			var configWithMissingIterationsProperty = "{\r\n  \"Id\": 1,\r\n  \"Name\": \"Test_Simulation\",\r\n  \"Comments\": \"unit test\",\r\n    \"IterationStart\": 0,\r\n    \"TimeOutPeriod\": 10,\r\n    \"Seed\": 1234,\r\n    \"Quality\": 3,\r\n    \"FotooptikQuality\": 2\r\n}";

			IList<string> errors = new List<string>();
			var result = Serializer<SimulationConfiguration>.Validate(configWithMissingIterationsProperty, out errors);

			Assert.IsFalse(result);
			Assert.AreEqual(1, errors.Count);
			StringAssert.StartsWith("Required properties are missing from object: Iterations.", errors[0]);
		}
	}
}
