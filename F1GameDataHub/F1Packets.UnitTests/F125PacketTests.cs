using F1Packets.F125;
using System.Runtime.InteropServices;

namespace F1Packets.UnitTests
{
    [TestClass]
    public sealed class F125PacketTests
    {
        [TestMethod]
        public void PacketMotionData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\motion_packet.bin");
            var packet = GetPacketFromBytes<PacketMotionData>(bytes);
        }

        [TestMethod]
        public void PacketSessionData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\session_packet.bin");
            var packet = GetPacketFromBytes<PacketSessionData>(bytes);
        }

        [TestMethod]
        public void PacketLapData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\lap_packet.bin");
            var packet = GetPacketFromBytes<PacketLapData>(bytes);
        }

        [TestMethod]
        public void PacketEventData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\event_packet.bin");
            var packet = GetPacketFromBytes<PacketEventData>(bytes);
        }

        [TestMethod]
        public void PacketParticipantsData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\participants_packet.bin");
            var packet = GetPacketFromBytes<PacketParticipantsData>(bytes);
        }

        [TestMethod]
        public void PacketCarSetupsData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\car_setups_packet.bin");
            var packet = GetPacketFromBytes<PacketCarSetupsData>(bytes);
        }

        [TestMethod]
        public void PacketCarTelemetryData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\car_telemetry_packet.bin");
            var packet = GetPacketFromBytes<PacketCarTelemetryData>(bytes);
        }

        [TestMethod]
        public void PacketCarStatusData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\car_status_packet.bin");
            var packet = GetPacketFromBytes<PacketCarStatusData>(bytes);
        }

        [TestMethod]
        public void PacketFinalClassificationData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\final_classification_packet.bin");
            var packet = GetPacketFromBytes<PacketFinalClassificationData>(bytes);
        }

        [TestMethod]
        public void PacketLobbyInfoData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\lobby_info_packet.bin");
            var packet = GetPacketFromBytes<PacketLobbyInfoData>(bytes);
        }

        [TestMethod]
        public void PacketCarDamageData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\car_damage_packet.bin");
            var packet = GetPacketFromBytes<PacketCarDamageData>(bytes);
        }

        [TestMethod]
        public void PacketSessionHistoryData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\session_history_packet.bin");
            var packet = GetPacketFromBytes<PacketSessionHistoryData>(bytes);
        }

        [TestMethod]
        public void PacketTyreSetsData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\tyre_sets_packet.bin");
            var packet = GetPacketFromBytes<PacketTyreSetsData>(bytes);
        }

        [TestMethod]
        public void PacketMotionExData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\motion_ex_packet.bin");
            var packet = GetPacketFromBytes<PacketMotionExData>(bytes);
        }

        [TestMethod]
        public void PacketTimeTrialData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\time_trial_packet.bin");
            var packet = GetPacketFromBytes<PacketTimeTrialData>(bytes);
        }

        [TestMethod]
        public void PacketLapPositionsData_Test()
        {
            var bytes = File.ReadAllBytes(@"TestData\F125\lap_positions_packet.bin");
            var packet = GetPacketFromBytes<PacketLapPositionsData>(bytes);
        }

        private static T GetPacketFromBytes<T>(byte[] bytes) where T : struct
        {
            var expectedSize = Marshal.SizeOf<T>();
            Assert.AreEqual(expectedSize, bytes.Length, $"Packet size does not match expected size for {typeof(T).Name}.");
            return Utils.ReadFromBytes<T>(bytes);
        }
    }
}
