namespace PcBuilder.Classes
{
    public partial class Models
    {
        public static string CpuTable = @"
            CREATE TABLE IF NOT EXISTS cpu (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                core_count INT,
                core_clock TEXT,
                boost_clock TEXT,
                tdp INT,
                graphics TEXT,
                smt BOOLEAN,
                socket TEXT
            );";

        public static string MotherboardTable = @"
            CREATE TABLE IF NOT EXISTS motherboard (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                socket TEXT,
                form_factor TEXT,
                max_memory INT,
                memory_slots INT,
                color TEXT
            );";

        public static string MemoryTable = @"
            CREATE TABLE IF NOT EXISTS memory (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                speed TEXT,
                modules TEXT,
                price_per_gb TEXT,
                color TEXT,
                first_word_latency TEXT,
                cas_latency TEXT
            );";

        public static string PowerSupplyTable = @"
            CREATE TABLE IF NOT EXISTS power_supply (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                type TEXT,
                efficiency TEXT,
                wattage INT,
                modular TEXT,
                color TEXT
            );";

        public static string VideoCardTable = @"
            CREATE TABLE IF NOT EXISTS video_card (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                chipset TEXT,
                memory TEXT,
                core_clock TEXT,
                boost_clock TEXT,
                color TEXT,
                length TEXT
            );";

        public static string CaseTable = @"
            CREATE TABLE IF NOT EXISTS ""case"" (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                type TEXT,
                color TEXT,
                psu TEXT,
                side_panel TEXT,
                external_volume TEXT,
                internal_35_bays INT
            );";

        public static string InternalHardDriveTable = @"
            CREATE TABLE IF NOT EXISTS internal_hard_drive (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                capacity TEXT,
                price_per_gb TEXT,
                type TEXT,
                cache TEXT,
                form_factor TEXT,
                interface TEXT
            );";

        public static string CpuCoolerTable = @"
            CREATE TABLE IF NOT EXISTS cpu_cooler (
                id SERIAL PRIMARY KEY,
                name TEXT,
                price TEXT,
                rpm TEXT,
                noise_level TEXT,
                color TEXT,
                size TEXT
            );";

        public static string OsTable = @"
            CREATE TABLE IF NOT EXISTS os (
                id SERIAL PRIMARY KEY,
                name TEXT,                
                price TEXT,
                mode TEXT,
                max_memory INTEGER
            );";

        public static string Users = @"
            CREATE TABLE IF NOT EXISTS Users (
                id SERIAL PRIMARY KEY,
                login TEXT,
                password TEXT
            );";
    }
}
