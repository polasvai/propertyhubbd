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
            var locationData = new Dictionary<string, Dictionary<string, string[]>>
            {
                { "Dhaka", new Dictionary<string, string[]> {
                    { "Manikganj", new[] { "Harirampur", "Saturia", "Manikganj Sadar", "Gior", "Shibaloy", "Doulatpur", "Singiar" } },
                    { "Munshiganj", new[] { "Munshiganj Sadar", "Sreenagar", "Sirajdikhan", "Louhajanj", "Gajaria", "Tongibari" } },
                    { "Narsingdi", new[] { "Belabo", "Monohardi", "Narsingdi Sadar", "Palash", "Raipura", "Shibpur" } },
                    { "Gazipur", new[] { "Kaliganj", "Kaliakair", "Kapasia", "Gazipur Sadar", "Sreepur" } },
                    { "Faridpur", new[] { "Faridpur Sadar", "Alfadanga", "Boalmari", "Sadarpur", "Nagarkanda", "Bhanga", "Charbhadrasan", "Madhukhali", "Saltha" } },
                    { "Shariatpur", new[] { "Shariatpur Sadar", "Naria", "Zajira", "Gosairhat", "Bhedarganj", "Damudya" } },
                    { "Kishoreganj", new[] { "Itna", "Katiadi", "Bhairab", "Tarail", "Hossainpur", "Pakundia", "Kuliarchar", "Kishoreganj Sadar", "Karimgonj", "Bajitpur", "Austagram", "Mithamoin", "Nikli" } },
                    { "Narayanganj", new[] { "Araihazar", "Bandar", "Narayanganj Sadar", "Rupganj", "Sonargaon" } },
                    { "Rajbari", new[] { "Rajbari Sadar", "Goalanda", "Pangsa", "Baliakandi", "Kalukhali" } },
                    { "Dhaka", new[] { "Savar", "Dhamrai", "Keraniganj", "Nawabganj", "Dohar" } },
                    { "Tangail", new[] { "Basail", "Bhuapur", "Delduar", "Ghatail", "Gopalpur", "Madhupur", "Mirzapur", "Nagarpur", "Sakhipur", "Tangail Sadar", "Kalihati", "Dhanbari" } },
                    { "Gopalganj", new[] { "Gopalganj Sadar", "Kashiani", "Tungipara", "Kotalipara", "Muksudpur" } },
                    { "Madaripur", new[] { "Madaripur Sadar", "Shibchar", "Kalkini", "Rajoir", "Dasar" } },
                }},
                { "Barisal", new Dictionary<string, string[]> {
                    { "Jhalakathi", new[] { "Jhalakathi Sadar", "Kathalia", "Nalchity", "Rajapur" } },
                    { "Barguna", new[] { "Amtali", "Barguna Sadar", "Betagi", "Bamna", "Pathorghata", "Taltali" } },
                    { "Pirojpur", new[] { "Pirojpur Sadar", "Nazirpur", "Kawkhali", "Zianagar", "Bhandaria", "Mathbaria", "Nesarabad" } },
                    { "Patuakhali", new[] { "Bauphal", "Patuakhali Sadar", "Dumki", "Dashmina", "Kalapara", "Mirzaganj", "Galachipa", "Rangabali" } },
                    { "Bhola", new[] { "Bhola Sadar", "Borhan Sddin", "Charfesson", "Doulatkhan", "Monpura", "Tazumuddin", "Lalmohan" } },
                    { "Barisal", new[] { "Barisal Sadar", "Bakerganj", "Babuganj", "Wazirpur", "Banaripara", "Gournadi", "Agailjhara", "Mehendiganj", "Muladi", "Hizla" } },
                }},
                { "Khulna", new Dictionary<string, string[]> {
                    { "Narail", new[] { "Narail Sadar", "Lohagara", "Kalia" } },
                    { "Khulna", new[] { "Paikgasa", "Fultola", "Digholia", "Rupsha", "Terokhada", "Dumuria", "Botiaghata", "Dakop", "Koyra" } },
                    { "Meherpur", new[] { "Mujibnagar", "Meherpur Sadar", "Gangni" } },
                    { "Jashore", new[] { "Manirampur", "Abhaynagar", "Bagherpara", "Chougachha", "Jhikargacha", "Keshabpur", "Jessore Sadar", "Sharsha" } },
                    { "Bagerhat", new[] { "Fakirhat", "Bagerhat Sadar", "Mollahat", "Sarankhola", "Rampal", "Morrelganj", "Kachua", "Mongla", "Chitalmari" } },
                    { "Jhenaidah", new[] { "Jhenaidah Sadar", "Shailkupa", "Harinakundu", "Kaliganj", "Kotchandpur", "Moheshpur" } },
                    { "Kushtia", new[] { "Kushtia Sadar", "Kumarkhali", "Khoksa", "Mirpur", "Daulatpur", "Bheramara" } },
                    { "Satkhira", new[] { "Assasuni", "Debhata", "Kalaroa", "Satkhira Sadar", "Shyamnagar", "Tala", "Kaliganj" } },
                    { "Magura", new[] { "Shalikha", "Sreepur", "Magura Sadar", "Mohammadpur" } },
                    { "Chuadanga", new[] { "Chuadanga Sadar", "Alamdanga", "Damurhuda", "Jibannagar" } },
                }},
                { "Chattagram", new Dictionary<string, string[]> {
                    { "Bandarban", new[] { "Bandarban Sadar", "Alikadam", "Naikhongchhari", "Rowangchhari", "Lama", "Ruma", "Thanchi" } },
                    { "Khagrachhari", new[] { "Khagrachhari Sadar", "Dighinala", "Panchari", "Laxmichhari", "Mohalchari", "Manikchari", "Ramgarh", "Matiranga", "Guimara" } },
                    { "Chandpur", new[] { "Haimchar", "Kachua", "Shahrasti", "Chandpur Sadar", "Matlab South", "Hajiganj", "Matlab North", "Faridgonj" } },
                    { "Rangamati", new[] { "Rangamati Sadar", "Kaptai", "Kawkhali", "Baghaichari", "Barkal", "Langadu", "Rajasthali", "Belaichari", "Juraichari", "Naniarchar" } },
                    { "Brahmanbaria", new[] { "Brahmanbaria Sadar", "Kasba", "Nasirnagar", "Sarail", "Ashuganj", "Akhaura", "Nabinagar", "Bancharampur", "Bijoynagar" } },
                    { "Coxsbazar", new[] { "Coxsbazar Sadar", "Chakaria", "Kutubdia", "Ukhiya", "Moheshkhali", "Pekua", "Ramu", "Teknaf", "Eidgaon" } },
                    { "Comilla", new[] { "Debidwar", "Barura", "Brahmanpara", "Chandina", "Chauddagram", "Daudkandi", "Homna", "Laksam", "Muradnagar", "Nangalkot", "Comilla Sadar", "Meghna", "Monohargonj", "Sadarsouth", "Titas", "Burichang", "Lalmai" } },
                    { "Noakhali", new[] { "Noakhali Sadar", "Companiganj", "Begumganj", "Hatia", "Subarnachar", "Kabirhat", "Senbug", "Chatkhil", "Sonaimori" } },
                    { "Lakshmipur", new[] { "Lakshmipur Sadar", "Kamalnagar", "Raipur", "Ramgati", "Ramganj" } },
                    { "Chattogram", new[] { "Rangunia", "Sitakunda", "Mirsharai", "Patiya", "Sandwip", "Banshkhali", "Boalkhali", "Anwara", "Chandanaish", "Satkania", "Lohagara", "Hathazari", "Fatikchhari", "Raozan", "Karnafuli" } },
                    { "Feni", new[] { "Chhagalnaiya", "Feni Sadar", "Sonagazi", "Fulgazi", "Parshuram", "Daganbhuiyan" } },
                }},
                { "Sylhet", new Dictionary<string, string[]> {
                    { "Moulvibazar", new[] { "Barlekha", "Kamolganj", "Kulaura", "Moulvibazar Sadar", "Rajnagar", "Sreemangal", "Juri" } },
                    { "Sunamganj", new[] { "Sunamganj Sadar", "South Sunamganj", "Bishwambarpur", "Chhatak", "Jagannathpur", "Dowarabazar", "Tahirpur", "Dharmapasha", "Jamalganj", "Shalla", "Derai", "Madhyanagar" } },
                    { "Sylhet", new[] { "Balaganj", "Beanibazar", "Bishwanath", "Companiganj", "Fenchuganj", "Golapganj", "Gowainghat", "Jaintiapur", "Kanaighat", "Sylhet Sadar", "Zakiganj", "Dakshinsurma", "Osmaninagar" } },
                    { "Habiganj", new[] { "Nabiganj", "Bahubal", "Ajmiriganj", "Baniachong", "Lakhai", "Chunarughat", "Habiganj Sadar", "Madhabpur" } },
                }},
                { "Rangpur", new Dictionary<string, string[]> {
                    { "Nilphamari", new[] { "Syedpur", "Domar", "Dimla", "Jaldhaka", "Kishorganj", "Nilphamari Sadar" } },
                    { "Rangpur", new[] { "Rangpur Sadar", "Gangachara", "Taragonj", "Badargonj", "Mithapukur", "Pirgonj", "Kaunia", "Pirgacha" } },
                    { "Dinajpur", new[] { "Nawabganj", "Birganj", "Ghoraghat", "Birampur", "Parbatipur", "Bochaganj", "Kaharol", "Fulbari", "Dinajpur Sadar", "Hakimpur", "Khansama", "Birol", "Chirirbandar" } },
                    { "Lalmonirhat", new[] { "Lalmonirhat Sadar", "Kaliganj", "Hatibandha", "Patgram", "Aditmari" } },
                    { "Gaibandha", new[] { "Sadullapur", "Gaibandha Sadar", "Palashbari", "Saghata", "Gobindaganj", "Sundarganj", "Phulchari" } },
                    { "Panchagarh", new[] { "Panchagarh Sadar", "Debiganj", "Boda", "Atwari", "Tetulia" } },
                    { "Kurigram", new[] { "Kurigram Sadar", "Nageshwari", "Bhurungamari", "Phulbari", "Rajarhat", "Ulipur", "Chilmari", "Rowmari", "Charrajibpur" } },
                    { "Thakurgaon", new[] { "Thakurgaon Sadar", "Pirganj", "Ranisankail", "Haripur", "Baliadangi" } },
                }},
                { "Mymensingh", new Dictionary<string, string[]> {
                    { "Sherpur", new[] { "Sherpur Sadar", "Nalitabari", "Sreebordi", "Nokla", "Jhenaigati" } },
                    { "Jamalpur", new[] { "Jamalpur Sadar", "Melandah", "Islampur", "Dewangonj", "Sarishabari", "Madarganj", "Bokshiganj" } },
                    { "Netrokona", new[] { "Barhatta", "Durgapur", "Kendua", "Atpara", "Madan", "Khaliajuri", "Kalmakanda", "Mohongonj", "Purbadhala", "Netrokona Sadar" } },
                    { "Mymensingh", new[] { "Fulbaria", "Trishal", "Bhaluka", "Muktagacha", "Mymensingh Sadar", "Dhobaura", "Phulpur", "Haluaghat", "Gouripur", "Gafargaon", "Iswarganj", "Nandail", "Tarakanda" } },
                }},
                { "Rajshahi", new Dictionary<string, string[]> {
                    { "Naogaon", new[] { "Mohadevpur", "Badalgachi", "Patnitala", "Dhamoirhat", "Niamatpur", "Manda", "Atrai", "Raninagar", "Naogaon Sadar", "Porsha", "Sapahar" } },
                    { "Rajshahi", new[] { "Paba", "Durgapur", "Mohonpur", "Charghat", "Puthia", "Bagha", "Godagari", "Tanore", "Bagmara" } },
                    { "Natore", new[] { "Natore Sadar", "Singra", "Baraigram", "Bagatipara", "Lalpur", "Gurudaspur", "Naldanga" } },
                    { "Pabna", new[] { "Sujanagar", "Ishurdi", "Bhangura", "Pabna Sadar", "Bera", "Atghoria", "Chatmohar", "Santhia", "Faridpur" } },
                    { "Chapainawabganj", new[] { "Chapainawabganj Sadar", "Gomostapur", "Nachol", "Bholahat", "Shibganj" } },
                    { "Bogura", new[] { "Kahaloo", "Bogra Sadar", "Shariakandi", "Shajahanpur", "Dupchanchia", "Adamdighi", "Nondigram", "Sonatala", "Dhunot", "Gabtali", "Sherpur", "Shibganj" } },
                    { "Sirajganj", new[] { "Belkuchi", "Chauhali", "Kamarkhand", "Kazipur", "Raigonj", "Shahjadpur", "Sirajganj Sadar", "Tarash", "Ullapara" } },
                    { "Joypurhat", new[] { "Akkelpur", "Kalai", "Khetlal", "Panchbibi", "Joypurhat Sadar" } },
                }},
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
