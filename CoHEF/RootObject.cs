using System.Collections.Generic;

namespace CoHEF
{
    // This has to match the pipeline.dat lua table:
    // "lua_name" = { "steamachievement_name", "achievement_status" }
    // feld_steiner = { "landser_elite", "0" }
    // As an example here is the EF table:
    /*
    achievements_tbl = {
		    feld_steiner = { "landser_elite", "0" },
		    hitler_cats = { "fuhrer_katzen", "0" },
		    red_army = { "red_wave", "0" },
		    woroshilov = { "woroshilov_kv2", "0" },
		    saizew = { "saizew_sniper", "0" },
		    danko = { "danko_callin", "0" },
		    isu = { "isu_jager", "0" },
		    grup_steiner = { "angriff_steiner", "0" },
		    wittmann = { "wittmann_tiger", "0" },
		    soviet_prod = { "soviet_industry", "0" },
		    kv1 = { "kolobanov", "0" },
		    oneman = { "onemanarmy", "0" },
		    jeep = { "jeepconstruction", "0" },
		    bigcats = { "bigcats", "0" },
		    furysherman = { "bradpittfury", "0" },
		    furytiger = { "bradpitttiger", "0" },
		    rushberlin = { "rushberlin", "0" },
		    mohairborne = { "medalofhonorairborne", "0" },
		    mg42 = { "mg42", "0" },
		    collateral = { "collateral", "0" },
		    stransky = { "stransky", "0" },
		    is2construction = { "is2construction", "0" },
		    jungleking = { "jungleking", "0" },
		    stormtiger = { "stormtiger", "0" },
		    pershing = { "pershing", "0" },
		    hummel = { "hummel", "0" },
		    lendlease = { "lendlease", "0" },
		    volkssturm = { "volkssturm", "0" },
		    compstomp = { "compstomp", "0" },
		    pershing2 = { "pershing2", "0" },
		    churchill = { "churchill", "0" },
		    wolverine = { "wolverine", "0" },
		    crazywilly = { "crazywilly", "0" },
		    bergetiger = { "bergetiger", "0" },
		    kettenkrad = { "kettenkrad", "0" },
		    beutepanzer = { "beutepanzer", "0" },
		    bledforthis = { "bledforthis", "0" },
		    downfall = { "downfall", "0" },
		    r_btn = { "no_reinforce", "0" },
		    heroes = { "su_heroes", "0" },
		    vet4 = { "oh_heroes", "0" }
	    }
    */
    public class RootObject
    {
        public List<string> feld_steiner { get; set; }
        public List<string> churchill { get; set; }
        public List<string> woroshilov { get; set; }
        public List<string> hitler_cats { get; set; }
        public List<string> downfall { get; set; }
        public List<string> red_army { get; set; }
        public List<string> compstomp { get; set; }
        public List<string> mohairborne { get; set; }
        public List<string> jungleking { get; set; }
        public List<string> volkssturm { get; set; }
        public List<string> danko { get; set; }
        public List<string> pershing2 { get; set; }
        public List<string> collateral { get; set; }
        public List<string> vet4 { get; set; }
        public List<string> kettenkrad { get; set; }
        public List<string> isu { get; set; }
        public List<string> r_btn { get; set; }
        public List<string> heroes { get; set; }
        public List<string> stransky { get; set; }
        public List<string> lendlease { get; set; }
        public List<string> bledforthis { get; set; }
        public List<string> crazywilly { get; set; }
        public List<string> is2construction { get; set; }
        public List<string> jeep { get; set; }
        public List<string> wolverine { get; set; }
        public List<string> bergetiger { get; set; }
        public List<string> beutepanzer { get; set; }
        public List<string> rushberlin { get; set; }
        public List<string> mg42 { get; set; }
        public List<string> hummel { get; set; }
        public List<string> kv1 { get; set; }
        public List<string> grup_steiner { get; set; }
        public List<string> pershing { get; set; }
        public List<string> stormtiger { get; set; }
        public List<string> soviet_prod { get; set; }
        public List<string> saizew { get; set; }
        public List<string> furytiger { get; set; }
        public List<string> wittmann { get; set; }
        public List<string> bigcats { get; set; }
        public List<string> oneman { get; set; }
        public List<string> furysherman { get; set; }
    }
}
