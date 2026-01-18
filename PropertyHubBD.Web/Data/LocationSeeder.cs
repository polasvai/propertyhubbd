using PropertyHubBD.Web.Models;
using System.Linq;

namespace PropertyHubBD.Web.Data
{
    public static class LocationSeeder
    {
        public static void SeedLocations(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Divisions are likely seeded by DbInitializer, but let's ensure we have IDs for them.
            // If divisions don't exist, we skip for now (or could add them).
            var divisions = context.Divisions.ToList();
            if (!divisions.Any()) return;

            // Define Data Mapping: Division -> District -> Upazillas
            var locationData = new Dictionary<string, Dictionary<string, string[]>>()
            {
                { "Dhaka", new Dictionary<string, string[]> {
                    { "Dhaka", new[] { "Savar", "Dhamrai", "Keraniganj", "Nawabganj", "Dohar", "Dhaka Sadar" } },
                    { "Gazipur", new[] { "Gazipur Sadar", "Kaliakair", "Kaliganj", "Kapasia", "Sreepur" } },
                    { "Narayanganj", new[] { "Narayanganj Sadar", "Bandar", "Rupganj", "Sonargaon", "Araihazar" } },
                    { "Tangail", new[] { "Tangail Sadar", "Sakhipur", "Basail", "Madhupur", "Ghatail", "Kalihati", "Nagarpur", "Mirzapur", "Gopalpur", "Delduar", "Bhuapur", "Dhanbari" } },
                    { "Narsingdi", new[] { "Narsingdi Sadar", "Belabo", "Monohardi", "Palash", "Raipura", "Shibpur" } },
                    { "Faridpur", new[] { "Faridpur Sadar", "Boalmari", "Alfadanga", "Madhukhali", "Bhanga", "Nagarkanda", "Charbhadrasan", "Sadarpur", "Saltha" } }
                }},
                { "Chittagong", new Dictionary<string, string[]> {
                     { "Chittagong", new[] { "Chittagong Sadar", "Sitakunda", "Mirsharai", "Patiya", "Karnaphuli", "Lohagara", "Satkania", "Boalkhali", "Chandanaish", "Raozan", "Rangunia", "Fatikchhari", "Hathazari", "Anwara", "Banshkhali", "Sandwip" } },
                     { "Cox's Bazar", new[] { "Cox's Bazar Sadar", "Chakaria", "Kutubdia", "Ukhiya", "Moheshkhali", "Pekua", "Ramu", "Teknaf" } },
                     { "Comilla", new[] { "Comilla Sadar", "Barura", "Brahmanpara", "Burichang", "Chandina", "Chauddagram", "Daudkandi", "Debidwar", "Homna", "Laksam", "Muradnagar", "Nangalkot", "Meghna", "Titas", "Monohargonj", "Sadar South" } },
                     { "Feni", new[] { "Feni Sadar", "Chhagalnaiya", "Daganbhuiyan", "Parshuram", "Fulgazi", "Sonagazi" } },
                     { "Noakhali", new[] { "Noakhali Sadar", "Begumganj", "Chatkhil", "Companyganj", "Hatiya", "Senbagh", "Subarnachar", "Kabirhat", "Sonaimuri" } }
                }},
                { "Sylhet", new Dictionary<string, string[]> {
                     { "Sylhet", new[] { "Sylhet Sadar", "Beanibazar", "Bishwanath", "Dakshin Surma", "Balaganj", "Companiganj", "Fenchuganj", "Golapganj", "Gowainghat", "Jaintiapur", "Kanaighat", "Zakiganj", "Osmani Nagar" } },
                     { "Moulvibazar", new[] { "Moulvibazar Sadar", "Barlekha", "Juri", "Kamalganj", "Kulaura", "Rajnagar", "Sreemangal" } },
                     { "Habiganj", new[] { "Habiganj Sadar", "Azmiriganj", "Bahubal", "Baniyachong", "Chunarughat", "Lakhai", "Madhabpur", "Nabiganj", "Sayestaganj" } },
                     { "Sunamganj", new[] { "Sunamganj Sadar", "Bishwamvarpur", "Chhatak", "Derai", "Dharamapasha", "Dowarabazar", "Jagannathpur", "Jamalganj", "Sullah", "Tahirpur", "South Sunamganj", "Shantiganj" } }
                }},
                { "Rajshahi", new Dictionary<string, string[]> {
                     { "Rajshahi", new[] { "Rajshahi Sadar", "Bagha", "Bagmara", "Charghat", "Durgapur", "Godagari", "Mohanpur", "Paba", "Puthia", "Tanore" } },
                     { "Bogra", new[] { "Bogra Sadar", "Adamdighi", "Dhunat", "Dhupchanchia", "Gabtali", "Kahaloo", "Nandigram", "Sariakandi", "Sahajanpur", "Sherpur", "Shibganj", "Sonatala" } },
                      { "Pabna", new[] { "Pabna Sadar", "Atgharia", "Bera", "Bhangura", "Chatmohar", "Faridpur", "Ishwardi", "Santhia", "Sujanagar" } },
                      { "Sirajganj", new[] { "Sirajganj Sadar", "Belkuchi", "Chauhali", "Kamarkhanda", "Kazipur", "Raiganj", "Shahjadpur", "Tarash", "Ullapara" } }
                }},
                { "Khulna", new Dictionary<string, string[]> {
                     { "Khulna", new[] { "Khulna Sadar", "Batiaghata", "Dacope", "Dumuria", "Dighalia", "Koyra", "Paikgachha", "Phultala", "Rupsha", "Terokhada" } },
                     { "Jessore", new[] { "Jessore Sadar", "Abhaynagar", "Bagherpara", "Chaugachha", "Jhikargachha", "Keshabpur", "Manirampur", "Sharsha" } },
                     { "Satkhira", new[] { "Satkhira Sadar", "Assasuni", "Debhata", "Kalaroa", "Kaliganj", "Shyamnagar", "Tala" } }
                }},
                { "Barisal", new Dictionary<string, string[]> {
                     { "Barisal", new[] { "Barisal Sadar", "Agailjhara", "Babuganj", "Bakerganj", "Banaripara", "Gaurnadi", "Hizla", "Mehendiganj", "Muladi", "Wazirpur" } },
                     { "Patuakhali", new[] { "Patuakhali Sadar", "Bauphal", "Dashmina", "Galachipa", "Kalapara", "Mirzaganj", "Rangabali", "Dumki" } },
                     { "Bhola", new[] { "Bhola Sadar", "Burhanuddin", "Char Fasson", "Daulatkhan", "Lalmohan", "Manpura", "Tazumuddin" } }
                }},
                 { "Rangpur", new Dictionary<string, string[]> {
                     { "Rangpur", new[] { "Rangpur Sadar", "Badarganj", "Gangachara", "Kaunia", "Mithapukur", "Pirgachha", "Pirganj", "Taraganj" } },
                     { "Dinajpur", new[] { "Dinajpur Sadar", "Birampur", "Birganj", "Bochaganj", "Chirirbandar", "Fulbari", "Ghoraghat", "Hakimpur", "Kaharole", "Khansama", "Nawabganj", "Parbatipur" } }
                }},
                 { "Mymensingh", new Dictionary<string, string[]> {
                     { "Mymensingh", new[] { "Mymensingh Sadar", "Bhaluka", "Dhobaura", "Fulbaria", "Gafargaon", "Gauripur", "Haluaghat", "Ishwarganj", "Muktagachha", "Nandail", "Phulpur", "Trishal", "Tara Khanda" } },
                     { "Jamalpur", new[] { "Jamalpur Sadar", "Baksiganj", "Dewanganj", "Islampur", "Madarganj", "Melandaha", "Sarishabari" } }
                }}
            };

            foreach (var divData in locationData)
            {
                var divName = divData.Key;
                var division = divisions.FirstOrDefault(d => d.Name == divName);
                if (division == null) continue;

                foreach (var distData in divData.Value)
                {
                    var distName = distData.Key;
                    var upazillas = distData.Value;

                    // Add District
                    var district = context.Districts.FirstOrDefault(d => d.Name == distName && d.DivisionId == division.Id);
                    if (district == null)
                    {
                        district = new District { Name = distName, DivisionId = division.Id };
                        context.Districts.Add(district);
                        context.SaveChanges(); // Save to get Id
                    }

                    // Add Upazillas
                    foreach (var upzName in upazillas)
                    {
                        if (!context.Upazillas.Any(u => u.Name == upzName && u.DistrictId == district.Id))
                        {
                            context.Upazillas.Add(new Upazilla { Name = upzName, DistrictId = district.Id });
                        }
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
