using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;


public class ADRP : Script
{
    #region Definições
    // GreedyArg nos commands - é para quando usa frases em algum argumento

    // Banco de Dados
    private const string DB_SERVER = "localhost";
    private const string DB_NAME = "gtav";
    private const string DB_USER = "root";
    private const string DB_PASSWORD = "";
    private const string DB_PORT = "3306";
    private static MySqlConnection bancodados;


    // Definições
    private const int MAX_PLAYERS = 100;
    private const float DISTANCIA_RP = 10;
    private const int MAX_FACCOES = 10;
    private const int MAX_FACCOES_RANKS = 20;
    private const int MAX_PROPRIEDADES = 500;
    private const int TEMPO_ACEITAR_MORTE = 180000;
    private const int MAX_PERSONAGENS_VEICULOS = 30;
    private const int MAX_PERSONAGENS_PROPRIEDADES = 10;

    // Diretórios
    private const string DIR_LOGS = "logs";

    // Mensagens
    private const string MSG_SEM_AUTORIZACAO = "Você não possui autorização para utilizar esse comando.";
    private const string MSG_SEM_FACCAO = "Você não está em uma facção.";
    private const string MSG_OPCAO_INVALIDA = "Opção inválida.";

    // Logs
    private const string LOG_ENTRADAS = "entradas";
    private const string LOG_SAIDAS = "saidas";
    private const string LOG_KICKS = "kicks";
    private const string LOG_PAGAMENTOS = "pagamentos";
    private const string LOG_HEADADMIN = "headadmin";
    private const string LOG_PUNICOES = "punicoes";
    private const string LOG_UNBAN = "unban";

    // Cores
    private const string COR_PM_ENVIADA = "~#ECE641~";
    private const string COR_PM_RECEBIDA = "~#FBD918~";
    private const string COR_ROLEPLAY = "~#D0AEEB~";
    private const string COR_CHAT_OOC_GLOBAL = "~#AAC4E5~";
    private const string COR_CHAT_OOC_LOCAL = "~#AFAFAF~";
    private const string COR_STAFF = "~#33EE33~";
    private const string COR_RADIO = "~#FFFF9B~";
    private const string COR_MEGAFONE = "~#F5ED20~";
    private const string COR_PM_STAFF = "~#EB2626~";

    private string[] SKINS_PROIBIDAS_COMPRA =
        { "Autopsy01SMY", "BayWatch01SFY", "BayWatch01SMY", "BlackOps01SMY", "BlackOps02SMY", "BlackOps03SMY", "Boar", "BradCadaverCutscene", "Cat", "ChickenHawk", "Chimp", "Chop", "CIASec01SMM", "Claude01",
        "Cop01SFY", "Cop01SMY", "CopCutscene", "Cormorant", "Cow", "Coyote", "Crow", "Deer", "DoaMan", "Doctor01SMM", "FIBArchitect", "FIBOffice01SMM", "FIBSec01", "FIBSec01SMM", "Fish", "HammerShark", "Hen",
        "Humpback", "Husky", "HWayCop01SMY", "JohnnyKlebitz", "KarenDaniels", "KarenDanielsCutscene", "KillerWhale", "LamarDavisCutscene", "Marine01SMM", "Marine01SMY", "Marine02SMM", "Marine02SMY", "Marine03SMY",
        "Marston01", "MerryWeatherCutscene", "Michelle", "MichelleCutscene", "Misty01", "MountainLion", "NervousRonCutscene", "Niko01", "Paramedic01SMM", "Pig", "Pigeon", "Pogo01", "Poodle", "PrologueSec01",
        "PrologueSec01Cutscene", "PrologueSec02", "PrologueSec02Cutscene", "Pug", "Rabbit", "RampMarineCutscene", "Ranger01SFY", "Ranger01SMY", "Rat", "Retriever", "Rhesus", "Rottweiler", "Scientist01SMM",
        "Scrubs01SFY", "Seagull", "Sheriff01SFY", "Sheriff01SMY", "Shepherd", "SnowCop01SMM", "SteveHainsCutscene", "Stingray", "SWAT01SMY", "TigerShark", "TrafficWarden", "TrafficWardenCutscene", "WadeCutscene", "Westy" };
    private string[] SKINS_PROIBIDAS_SET =
        { "Boar", "BradCadaverCutscene", "Cat", "ChickenHawk", "Chimp", "Chop", "Claude01", "Cormorant", "Cow", "Coyote", "Crow", "Deer", "Fish", "HammerShark", "Hen", "Humpback", "Husky", "JohnnyKlebitz",
        "KillerWhale", "LamarDavisCutscene", "Marston01", "Misty01", "MountainLion", "NervousRonCutscene", "Niko01", "Pig", "Pigeon", "Poodle", "Pug", "Rabbit", "Rat", "Retriever", "Rhesus", "Rottweiler",  "Seagull",
        "Shepherd", "SteveHainsCutscene", "Stingray", "TigerShark", "TrafficWarden", "WadeCutscene", "Westy" };

    // Timers
    private Timer timerServidor = new Timer();

    private Timer timerSegundos = new Timer();

    // Sistema de ID
    private List<Client> players = new List<Client>();
    private Dictionary<Client, NetHandle> playerLabels = new Dictionary<Client, NetHandle>();

    // Listas
    private List<Faccao> faccoes = new List<Faccao>();
    private List<Propriedade> propriedades = new List<Propriedade>();
    private List<Veiculo> veiculos = new List<Veiculo>();
    private List<SOS> listaSOS = new List<SOS>();
    private List<Armario> armarios = new List<Armario>();
    private List<adBlip> blips = new List<adBlip>();
    private List<ATM> atms = new List<ATM>();
    private List<adPed> peds = new List<adPed>();
    private List<Comando> comandos = new List<Comando>();
    private List<Ponto> pontos = new List<Ponto>();
    private List<Concessionaria> concessionarias = new List<Concessionaria>();
    private List<Concessionaria.Veiculo> veiculosConce = new List<Concessionaria.Veiculo>();

    //Itens Dropados
    private List<ItensDropados> itensDropados = new List<ItensDropados>();

    //Cachorros
    private List<Pet> pets = new List<Pet>();

    // Valores $$$
    private const int VALOR_SALARIO = 350;
    private const int VALOR_SKIN = 200;
    private const int VALOR_SKIN_VARIACAO = 150;
    #endregion

    #region Classes / Enums
    public class CWeaponData
    {
        public int Ammo { get; set; }
        public WeaponTint Tint { get; set; }
        public string Components { get; set; }
    }

    [Flags]
    public enum AnimationFlags
    {
        Loop = 1 << 0,
        StopOnLastFrame = 1 << 1,
        OnlyAnimateUpperBody = 1 << 4,
        AllowPlayerControl = 1 << 5,
        Cancellable = 1 << 7
    }

    /* Tipos De Convite 
    * 1 - Facção 
    * 2 - Venda de Propriedade
    */

    /*
    STAFF
    Tester = 1,
    GameAdmin1 = 2,
    GameAdmin2 = 3,
    GameAdmin3 = 4,
    LeadAdmin = 5,
    HeadAdmin = 6,
    Developer = 7,
    Management = 8
    */

    //Portões de Facções
    public int PortaoLSPD_Fundos;
    public int PortaoLSPD_Fundos_Status = 1;

    public class Faccao
    {
        public int ID = 0;
        public string NomeFaccao = string.Empty;
        public string AbreviaturaFaccao = string.Empty;
        public Tipo TipoFaccao = Tipo.Civil;
        public string CorFaccao = string.Empty;
        public int IDRankGestor = MAX_FACCOES_RANKS;
        public int IDRankLider = MAX_FACCOES_RANKS;
        public List<Rank> Ranks = new List<Rank>();
        public bool ChatBloqueado = false;

        public class Rank
        {
            public int ID = 0;
            public string NomeRank = string.Empty;
            public int Salario = 0;
        }

        public enum Tipo
        {
            Civil = 1,
            Policial = 2,
            Bombeiros = 3,
            Governamental = 4,
            Criminal = 5,
        }
    }

    public class Pet
    {
        public int ID = 0;
        public string Nome = string.Empty;
        public PedHash Skin = new PedHash();
        public Ped Ped = null;
        public string NomePersonagemProprietario = string.Empty;
        public int IDPersonagemProprietario = 0;
        public int Sentado = 0;
        public int Dimensao = 0;
        public int Seguindo = 0;
        public Vector3 Posicao = new Vector3();

    }

    public class Propriedade
    {
        public int ID = 0;
        public Tipo TipoPropriedade = Tipo.Residencia;
        public int Interior = 0;
        public int Level = 1;
        public string Endereco = string.Empty;
        public int ValorPropriedade = 0;
        public int IDPersonagemProprietario = 0;
        public Vector3 EntradaFrente = new Vector3();
        public Vector3 SaidaFrente = new Vector3();
        public Vector3 EntradaFundos = new Vector3();
        public Vector3 SaidaFundos = new Vector3();
        public TextLabel TextFrente = null;
        public TextLabel TextValorFrente = null;
        public TextLabel TextFundos = null;
        public TextLabel TextValorFundos = null;
        public bool StatusPortaFrente = false;
        public bool StatusPortaFundos = false;
        public Marker MarkerFrente = null;
        public Marker MarkerFundos = null;
        public int IDPropriedade = 0;
        public string NomePersonagemProprietario = string.Empty;
        public int IDFaccao = 0;

        public enum Tipo
        {
            Residencia = 1,
            Predio = 2,
        }
    }

    public class Veiculo
    {
        public Vehicle Veh = null;
        public int ID = 0;
        public string Modelo = string.Empty;
        public Vector3 Posicao = new Vector3();
        public Vector3 Rotacao = new Vector3();

        public GrandTheftMultiplayer.Server.Constant.Color Cor1 = new GrandTheftMultiplayer.Server.Constant.Color();
        public GrandTheftMultiplayer.Server.Constant.Color Cor2 = new GrandTheftMultiplayer.Server.Constant.Color();

        public float Vida = 0;
        public int IDPersonagemProprietario = 0;
        public int IDFaccao = 0;
        public string Placa = string.Empty;
        public Vector3 PosicaoSpawn = new Vector3();
        public int Explodido = 0;
        public float gasolina = 0;

        public string Inv1 = string.Empty;
        public string Inv2 = string.Empty;
        public string Inv3 = string.Empty;
        public string Inv4 = string.Empty;
        public string Inv5 = string.Empty;
        public string Inv6 = string.Empty;
        public string Inv7 = string.Empty;
        public string Inv8 = string.Empty;
        public string Inv9 = string.Empty;
        public string Inv10 = string.Empty;
        public int Inv1_q = 0;
        public int Inv2_q = 0;
        public int Inv3_q = 0;
        public int Inv4_q = 0;
        public int Inv5_q = 0;
        public int Inv6_q = 0;
        public int Inv7_q = 0;
        public int Inv8_q = 0;
        public int Inv9_q = 0;
        public int Inv10_q = 0;
    }

    public class Armario
    {
        public int ID = 0;
        public Vector3 Posicao = new Vector3();
        public int Dimensao = 0;
        public int IDFaccao = 0;
        public int IDRank = 0;
        public TextLabel Text = null;
        public List<Item> Itens = new List<Item>();

        public class Item
        {
            public string Arma = string.Empty;
            public int Municao = 0;
            public int Estoque = 0;
            public int Pintura = 0;
            public int IDRank = 0;
            public string Componentes = string.Empty;
        }
    }

    public class SOS
    {
        public int IDPlayer = 0;
        public string NomePersonagem = string.Empty;
        public string Descricao = string.Empty;
    }

    public class adBlip
    {
        public int ID = 0;
        public Vector3 Posicao = new Vector3();
        public string Nome = string.Empty;
        public int Tipo = 0;
        public int Cor = 0;
        public Blip Blip = null;
    }

    public class ATM
    {
        public int ID = 0;
        public Vector3 Posicao = new Vector3();
        public TextLabel Text = null;
    }

    public class adPed
    {
        public int ID = 0;
        public Ped Ped = null;
        public Vector3 Posicao = new Vector3();
        public int Dimensao = 0;
        public int Rotacao = 0;
        public PedHash Skin = new PedHash();
    }

    public class Comando
    {
        public string Nome = string.Empty;
        public string Categoria = string.Empty;
        public string Descricao = string.Empty;
        public int Staff = 0;
    }

    public class Ponto
    {
        public int ID = 0;
        public Vector3 Posicao = new Vector3();
        public Tipo TipoPonto = 0;
        public string Descricao = string.Empty;
        public TextLabel Text = null;

        public enum Tipo
        {
            Skin = 1,
            Uniforme = 2,
        }
    }

    public class ItensDropados
    {
        public int ID = 0;
        public Vector3 Posicao = new Vector3();
        public Vector3 Rotacao = new Vector3();
        public Tipo TipoItem = 0;
        public string Modelo;
        public int Quantidade = 0;
        public NetHandle Objeto;

        public enum Tipo
        {
            Dinheiro = 1,
            Arma = 2,
        }
    }

    public class Concessionaria
    {
        public int ID = 0;
        public Vector3 Posicao = new Vector3();
        public string Tipo = string.Empty;
        public TextLabel Text = null;
        public Vector3 Posicao_vspawn = new Vector3();

        public class Veiculo
        {
            public string Categoria = string.Empty;
            public string Nome = string.Empty;
            public int Preco = 0;
            public int Velocidade = 0;
            public int Frenagem = 0;
            public int Aceleracao = 0;
            public int Tracao = 0;
        }
    }
    #endregion

    #region API
    public ADRP()
    {
        API.onResourceStart += API_onResourceStart;
        API.onResourceStop += API_onResourceStop;
        API.onChatMessage += API_onChatMessage;
        API.onPlayerDisconnected += API_onPlayerDisconnected;
        API.onPlayerDeath += API_onPlayerDeath;
        API.onPlayerRespawn += API_onPlayerRespawn;
        API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
        API.onClientEventTrigger += API_onClientEventTrigger;
        API.onChatCommand += API_onChatCommand;
        API.onPlayerConnected += API_onPlayerConnected;
        API.onPlayerHealthChange += API_onPlayerHealthChange;
        API.onPlayerEnterVehicle += OnPlayerEnterVehicle;
        API.onVehicleDeath += API_onVehicleDeath;
        API.onPlayerExitVehicle += API_onPlayerExitVehicle;

        API.onEntityEnterColShape += OnEntityEnterColShapeHandler;
        API.onPlayerWeaponSwitch += OnPlayerWeaponSwitchHandler;
    }

    private void OnPlayerWeaponSwitchHandler(Client player, WeaponHash oldWeapon)
    {
        if (!API.hasEntitySyncedData(player, "logado")) return;
        int return_w = RetornarHashArmaByName(oldWeapon.ToString());

        if (return_w != 99)
        {
            if (return_w != 0)
            {
                API.sendNativeToPlayer(player, Hash.SET_CURRENT_PED_WEAPON, player, return_w, 1);
            }
        }
        else
            EnviarMensagemErro(player, "deu ruim #0023");
    }

    public int RetornarHashArmaByName(String ArmaName)
    {
        int return_w = 0;
        switch (ArmaName)
        {
            //Armas Brancas
            case "Unarmed": return_w = 0; break;
            case "Knife": return_w = -1716189206; break;
            case "Nightstick": return_w = 1737195953; break;
            case "Hammer": return_w = 1317494643; break;
            case "Bat": return_w = -1786099057; break;
            case "Crowbar": return_w = -2067956739; break;
            case "Golfclub": return_w = 1141786504; break;
            case "Bottle": return_w = -102323637; break;
            case "Dagger": return_w = -1834847097; break;
            case "Hatchet": return_w = -102973651; break;
            case "KnuckleDuster": return_w = -656458692; break;
            case "Machete": return_w = -581044007; break;
            case "Flashlight": return_w = -1951375401; break;
            case "SwitchBlade": return_w = -538741184; break;
            case "Poolcue": return_w = -1810795771; break;
            case "Wrench": return_w = 419712736; break;
            case "Battleaxe": return_w = -853065399; break;
            //Pistolas
            case "Pistol": return_w = 453432689; break;
            case "CombatPistol": return_w = 1593441988; break;
            case "Pistol50": return_w = -1716589765; break;
            case "SNSPistol": return_w = -1076751822; break;
            case "HeavyPistol": return_w = -771403250; break;
            case "VintagePistol": return_w = 137902532; break;
            case "MarksmanPistol": return_w = -598887786; break;
            case "Revolver": return_w = -1045183535; break;
            case "APPistol": return_w = 584646201; break;
            case "StunGun": return_w = 911657153; break;
            case "FlareGun": return_w = 1198879012; break;
            //Machine Guns
            case "MicroSMG": return_w = 324215364; break;
            case "MachinePistol": return_w = -619010992; break;
            case "SMG": return_w = 736523883; break;
            case "AssaultSMG": return_w = -270015777; break;
            case "CombatPDW": return_w = 171789620; break;
            case "MG": return_w = -1660422300; break;
            case "CombatMG": return_w = -2144741730; break;
            case "Gusenberg": return_w = 1627465347; break;
            case "MiniSMG": return_w = -1121678507; break;
            //Assault Rifles
            case "AssaultRifle": return_w = -1074790547; break;
            case "CarbineRifle": return_w = -2084633992; break;
            case "AdvancedRifle": return_w = -1357824103; break;
            case "SpecialCarbine": return_w = -1063057011; break;
            case "BullpupRifle": return_w = 2132975508; break;
            case "CompactRifle": return_w = 1649403952; break;
            //Sniper Rifles
            case "SniperRifle": return_w = 100416529; break;
            case "HeavySniper": return_w = 205991906; break;
            case "MarksmanRifle": return_w = -952879014; break;
            //Shotguns
            case "PumpShotgun": return_w = 487013001; break;
            case "SawnoffShotgun": return_w = 2017895192; break;
            case "BullpupShotgun": return_w = -1654528753; break;
            case "AssaultShotgun": return_w = 494615257; break;
            case "Musket": return_w = -1466123874; break;
            case "HeavyShotgun": return_w = 984333226; break;
            case "DoubleBarrelShotgun": return_w = 275439685; break;
            case "SweeperShotgun": return_w = 317205821; break;
            //Heavy Weapons
            case "GrenadeLauncher": return_w = -1568386805; break;
            case "RPG": return_w = -1312131151; break;
            case "Minigun": return_w = 1119849093; break;
            case "Firework": return_w = 2138347493; break;
            case "Railgun": return_w = 1834241177; break;
            case "HomingLauncher": return_w = 1672152130; break;
            case "GrenadeLauncherSmoke": return_w = 1305664598; break;
            case "CompactLauncher": return_w = 125959754; break;
            //Thrown Weapons
            case "Grenade": return_w = -1813897027; break;
            case "StickyBomb": return_w = 741814745; break;
            case "ProximityMine": return_w = -1420407917; break;
            case "BZGas": return_w = -1600701090; break;
            case "Molotov": return_w = 615608432; break;
            case "FireExtinguisher": return_w = 101631238; break;
            case "PetrolCan": return_w = 883325847; break;
            case "Flare": return_w = 1233104067; break;
            case "Ball": return_w = 600439132; break;
            case "Snowball": return_w = 126349499; break;
            case "SmokeGrenade": return_w = -37975472; break;
            case "Pipebomb": return_w = -1169823560; break;
            //========================================
            default: return_w = 99; break;
        }
        return return_w;
    }

    [Command("dropg")]
    public void CMD_asdsadasdasd(Client player)
    {
        API.sendNativeToAllPlayers(Hash.SET_PED_DROPS_WEAPON, player);
    }
    [Command("reload")]
    public void CMD_asdsadaasdsdasd(Client player)
    {
        API.sendNativeToPlayer(player, Hash.MAKE_PED_RELOAD, player);
    }
    [Command("test1")]
    public void CMD_asdsadaasdssdasd(Client player)
    {
        API.sendNativeToPlayer(player, Hash.SET_WEAPON_SMOKEGRENADE_ASSIGNED, player);
    }

    public void API_onResourceStart()
    {
        API.setGamemodeName("adrp");
        API.setTime(DateTime.Now.Hour, DateTime.Now.Minute);
        API.setWeather(1);

        API.setWorldSyncedData("horario", string.Format("{0}:{1}", DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0')));

        timerServidor.Elapsed += new ElapsedEventHandler(timerServidor_Elapsed);
        timerServidor.Interval = 60000;
        timerServidor.Enabled = true;
        
        timerSegundos.Elapsed += new ElapsedEventHandler(timerSegundos_Elapsed);
        timerSegundos.Interval = 1000;
        timerSegundos.Enabled = true;

        for (var i = 0; i < MAX_PLAYERS; i++)
            players.Add(null);

        bancodados = new MySqlConnection(string.Format("server={0};user={1};database={2};password={3};port={4};", DB_SERVER, DB_USER, DB_NAME, DB_PASSWORD.Trim(), DB_PORT));
        bancodados.Open();
        if (bancodados.State.Equals(System.Data.ConnectionState.Open))
            API.consoleOutput("Conexão com o banco de dados estabelecida com sucesso!");

        API.consoleOutput("");
        API.consoleOutput("--------------------------------------");
        API.consoleOutput("      Advanced Roleplay Network       ");
        API.consoleOutput("  Desenvolvido por TR3V1Z4 And Freeze ");
        API.consoleOutput("                 2017                 ");
        API.consoleOutput("--------------------------------------");
        API.consoleOutput("");

        //================================================================================================================
        //                                          Abrir Portas
        //================================================================================================================

        int[] doorID = new int[10];

        //Loja de Roupas - Ganton (Atrás da casa do Fraklin)
        doorID[0] = API.exported.doormanager.registerDoor(-1148826190, new Vector3(82.692, -1390.920, 29.409));
        API.exported.doormanager.setDoorState(doorID[0], false, 0);

        doorID[1] = API.exported.doormanager.registerDoor(868499217, new Vector3(82.718, -1392.108, 29.397));
        API.exported.doormanager.setDoorState(doorID[1], false, 0);

        //Celas da LSPD
        doorID[2] = API.exported.doormanager.registerDoor(631614199, new Vector3(461.8065, -994.4086, 25.06443));
        API.exported.doormanager.setDoorState(doorID[2], true, 0);
        doorID[3] = API.exported.doormanager.registerDoor(631614199, new Vector3(461.8065, -997.6583, 25.06443));
        API.exported.doormanager.setDoorState(doorID[3], true, 0);
        doorID[4] = API.exported.doormanager.registerDoor(631614199, new Vector3(461.8065, -1001.302, 25.06443));
        API.exported.doormanager.setDoorState(doorID[4], true, 0);

        //Premium Deluxe Motorsport
        doorID[5] = API.exported.doormanager.registerDoor(2059227086, new Vector3(-59.89302, -1092.952, 26.88362));
        API.exported.doormanager.setDoorState(doorID[5], false, 0);
        doorID[6] = API.exported.doormanager.registerDoor(1417577297, new Vector3(-60.54582, -1094.749, 26.88872));
        API.exported.doormanager.setDoorState(doorID[6], false, 0);

        doorID[7] = API.exported.doormanager.registerDoor(2059227086, new Vector3(-39.13366, -1108.218, 26.7198));
        API.exported.doormanager.setDoorState(doorID[7], false, 0);
        doorID[8] = API.exported.doormanager.registerDoor(1417577297, new Vector3(-37.33113, -1108.873, 26.7198));
        API.exported.doormanager.setDoorState(doorID[8], false, 0);

        //Portao fundo da LSPD
        PortaoLSPD_Fundos = API.exported.doormanager.registerDoor(-1603817716, new Vector3(488.8923, -1011.67, 27.14583));
        API.exported.doormanager.transitionDoor(PortaoLSPD_Fundos, 0, 3000);
        PortaoLSPD_Fundos_Status = 1;
        API.exported.doormanager.setDoorState(PortaoLSPD_Fundos, true, 0);


        CarregarComandos();
        CarregarFaccoes();
        CarregarPropriedades();
        CarregarVeiculos();
        CarregarArmarios();
        CarregarBlips();
        CarregarATMs();
        CarregarPEDs();
        CarregarPontos();
        CarregarConcessionarias();
        CarregarItensDropados();
        CarregarPets();

        CriarIconsLojaDeRoupas();
        CriarEmprego_Taxista();
        CriandoEmprego_MotoristaDeOnibus();
        CriandoEmprego_ManutencaoCity();

        CriarAutoEscola();
        CriarPostosDeGasolina();

        //Mors Mutual Seguros
        API.createMarker(0, new Vector3(-202.2094, -1158.326, 23.81366), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 255, 255, 0);
        Blip MorsMutualBlip = API.createBlip(new Vector3(-202.2094, -1158.326, 23.81366), 300f);
        API.setBlipSprite(MorsMutualBlip, 380);
        API.setBlipColor(MorsMutualBlip, 0);
        API.setBlipName(MorsMutualBlip, "Mors Mutual Seguros.");
        API.setBlipShortRange(MorsMutualBlip, true);
        API.createTextLabel("[Mors Mutual Seguros]\nUse '/v remontar [id]'", new Vector3(-202.2094, -1158.326, 23.81366), 8f, 0.6f);

        
    }

    private void timerServidor_Elapsed(object source, ElapsedEventArgs e)
    {

        var ps = API.getAllPlayers();
        foreach (var player in ps)
        {
            if (!API.hasEntitySyncedData(player, "logado"))
                continue;

            var tempoConectado = API.getEntitySyncedData(player, "tempoconectado") + 1;
            API.setEntitySyncedData(player, "tempoconectado", tempoConectado);

            if (API.getEntitySyncedData(player, "atrabalho"))
                API.setEntitySyncedData(player, "tempoatrabalho", API.getEntitySyncedData(player, "tempoatrabalho") + 1);

            if (tempoConectado == 60)
            {
                // Paycheck do player
                API.setEntitySyncedData(player, "tempoconectado", 0);
                API.setEntitySyncedData(player, "xp", API.getEntitySyncedData(player, "xp") + 1);

                var paycheck = 0;

                if (API.getEntitySyncedData(player, "idfaccao") != 0)
                {
                    var rank = (Faccao.Rank)recuperarFaccaoRankPorID(API.getEntitySyncedData(player, "idfaccao"), API.getEntitySyncedData(player, "idrank"));
                    if (rank.Salario > 0)
                        paycheck += rank.Salario;
                    else
                        paycheck += VALOR_SALARIO;
                }
                else
                {
                    paycheck += VALOR_SALARIO;
                }

                var str = string.Empty;
                if (paycheck >= 0)
                    str = "~g~[+] $" + paycheck;
                else
                    str = "~r~[-] $" + paycheck;

                API.setEntitySyncedData(player, "banco", API.getEntitySyncedData(player, "banco") + paycheck);
                API.sendPictureNotificationToPlayer(player, str, "CHAR_BANK_FLEECA", 0, 0, "Pagamento", string.Empty);
            }

            var diff = DateTime.Now - DateTime.Parse(API.getEntitySyncedData(player, "datasalvamento"));
            if (diff.Minutes >= 5) // Salva os personagens a cada 5 minutos
            {
                SalvarPersonagem(player);
                API.setEntitySyncedData(player, "datasalvamento", DateTime.Now.ToString());
            }
        }
    }

    public void API_onResourceStop()
    {
        SalvarVeiculos();

        foreach (Client target in API.getAllPlayers())
            SalvarPersonagem(target);

        bancodados.Close();
    }

    public void API_onChatMessage(Client player, string message, CancelEventArgs e)
    {
        if (!API.hasEntitySyncedData(player, "logado"))
        {
            e.Cancel = true;
            return;
        }

        EnviarMensagemRadius(player, DISTANCIA_RP, recuperarNomeIC(player) + " diz: " + message);
        e.Cancel = true;
    }


    private void API_onPlayerDisconnected(Client player, string reason)
    {
        VerificarCoisasCriadas(player);
        Taxi_OnPlayerDisconnectedHandler(player);
        CarroEmprego_OnPlayerDisconnectedHandler(player);

        if (API.hasEntitySyncedData(player, "TemDog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") == 1)
            {
                API.setEntitySyncedData(player, "TemDog", 0);
                API.setEntitySyncedData(player, "mydog", null);
                API.setEntitySyncedData(player, "NomeDoDog", string.Empty);
                API.setEntitySyncedData(player, "Dog_ID", 0);


            }
        }

        var index = players.IndexOf(player);
        if (index != -1)
            players[index] = null;

        if (!API.hasEntitySyncedData(player, "logado")) return;

        EnviarMensagemEntradaSaida(player, "saiu");
        SalvarPersonagem(player);
        CriarLog(LOG_SAIDAS, string.Format("{0} ({1}) saiu do servidor ({2})", player.name, player.address, reason));
    }

    private void API_onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
    {
        API.sendNativeToPlayer(player, Hash._RESET_LOCALPLAYER_STATE, player);
        API.sendNativeToPlayer(player, Hash.RESET_PLAYER_ARREST_STATE, player);

        API.sendNativeToPlayer(player, Hash.IGNORE_NEXT_RESTART, true);
        API.sendNativeToPlayer(player, Hash._DISABLE_AUTOMATIC_RESPAWN, true);

        API.sendNativeToPlayer(player, Hash.SET_FADE_IN_AFTER_DEATH_ARREST, true);
        API.sendNativeToPlayer(player, Hash.SET_FADE_OUT_AFTER_DEATH, false);
        API.sendNativeToPlayer(player, Hash.NETWORK_REQUEST_CONTROL_OF_ENTITY, player);

        API.sendNativeToPlayer(player, Hash.FREEZE_ENTITY_POSITION, player, false);
        API.shared.sendNativeToPlayer(player, Hash.NETWORK_RESURRECT_LOCAL_PLAYER, player.position.X, player.position.Y, player.position.Z, player.rotation.Z, false, false);
        API.shared.sendNativeToPlayer(player, Hash.RESURRECT_PED, player);

        //========================================================================

        SetarPlayerFerido(player);

        /*if (API.hasEntitySyncedData(player, "Dono_DogMeAtacando"))
        {
            if (API.getEntitySyncedData(player, "Dono_DogMeAtacando") != null)
            {
                string idOrName = API.getEntitySyncedData(player, "Dono_DogMeAtacando");
                Pet pet_seguindo = recuperarPetPorID(API.getEntitySyncedData(player, "DogVindoAtacar"));

                var target = findPlayer(player, idOrName);
                if (target == null)
                {
                    API.sendNativeToAllPlayers(Hash.CLEAR_PED_TASKS, pet_seguindo.Ped);
                }
                else
                {
                    EnviarMensagemSucesso(target, "Fugiu do seu cachorro.");
                    API.sendNativeToAllPlayers(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY, pet_seguindo.Ped, target, 0, 0, 0, 1, -1, 1.0, true);
                    API.setEntitySyncedData(target, "DogIndoAtacar", "nc");
                }
                EnviarMensagemSucesso(player, "Você fugiu do cachorro.");
                API.setEntitySyncedData(player, "Dono_DogMeAtacando", null);
                API.setEntitySyncedData(player, "DogVindoAtacar", 0);
                API.setEntitySyncedData(player, "DogVindoMeAtacar", null);
            }
        }*/

    }

    private void API_onPlayerRespawn(Client player)
    {
        API.setEntityPosition(player, API.getEntitySyncedData(player, "posmorte"));

        updatePlayerFace(player);
    }

    private void API_onPlayerConnected(Client player)
    {
        var index = recuperarIDLivre();
        if (index != -1)
            players[index] = player;

        API.setEntityDimension(player, index + 1000);
        API.setEntitySyncedData(player, "idplayer", index);
        API.setEntitySyncedData(player, "dimensaorandom", index + 1000);
        API.setEntitySyncedData(player, "errosautenticacao", 0);

        API.setEntitySyncedData(player.handle, "AutoEscola_Teste", 0);
        API.setEntitySyncedData(player, "VelMax", 0);

        API.clearPlayerTasks(player);

        initializePedFace(player, 0);
        Taxi_OnPlayerConnectedHandler(player);
        DogSystem_Inicializar(player);

        API.setEntityTransparency(player, 0);
        API.freezePlayer(player, true);
        API.setEntityInvincible(player, true);
        API.setEntityPosition(player, new Vector3(-433.8039, 1148.86, 374.334));
        API.triggerClientEvent(player, "criarcamera", new Vector3(-433.8039, 1145.86, 374.334), new Vector3(-376.5278, 743.2303, 373.5411));
    }

    private void API_onPlayerFinishedDownload(Client player)
    {
        API.triggerClientEvent(player, "criarcamera", new Vector3(-433.8039, 1145.86, 374.334), new Vector3(-376.5278, 743.2303, 373.5411));

        API.sendNotificationToPlayer(player, "Você tem 80 segundos para se autenticar e escolher um personagem.");
        API.delay(80000, true, () =>
        {
            if (!API.hasEntitySyncedData(player, "logado"))
                player.kick("Você não autenticou em 80 segundos.");
        });

        API.triggerClientEvent(player, "player_login");
    }

    public void API_onChatCommand(Client player, string command, CancelEventArgs e)
    {
        if (!command.ToLower().Contains("login") && !API.hasEntitySyncedData(player, "logado"))
        {
            EnviarMensagemErro(player, "Você não está autenticado.");
            e.Cancel = true;
        }
    }


    private void API_onPlayerHealthChange(Client player, int oldValue)
    {
        //Em testes - Verificar funcionalidade
        /*if (player.health <= 10)
        {
            API.setPlayerHealth(player, 10);
        }*/
    }

    private void OnPlayerEnterVehicle(Client player, NetHandle vehicle, int targetSeat)
    {
        if (vehicle == null) return;
        var veh = recuperarVeiculo(vehicle);

        if (veh == null) return;

        API.setEntitySyncedData(player, "uvehid", veh.ID);

        if (veh.IDFaccao > 0)
        {
            if (!veh.IDFaccao.Equals(API.getEntitySyncedData(player, "idfaccao")) && player.vehicleSeat == -1)
            {
                //API.warpPlayerOutOfVehicle(player, vehicle);
                EnviarMensagemErro(player, "Você não pertence a essa facção.");
                return;
            }
        }
        
        /*if (API.hasEntitySyncedData(player, "mydog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") != 0)
            {
                var pedcc = (NetHandle)API.getEntitySyncedData(player, "mydog");

                //API.sendNativeToPlayer(player, Hash.SET_PED_INTO_VEHICLE, pedcc, vehicle, 1);
                //API.sendNotificationToPlayer(player, "Cachorro entrou no carro");
                
                API.sendNativeToPlayer(player, Hash.TASK_ENTER_VEHICLE, pedcc, vehicle, -1, 1, 1.0, 1, 0);


                API.delay(5000, true, () =>
                {
                    API.playPedScenario(pedcc, "WORLD_DOG_SITTING_ROTTWEILER");
                });
                
            }
        }*/

        if (veh.Modelo == "Taxi") Taxi_OnPlayerEnterVehicle(player, vehicle);

    }
    
    private void API_onPlayerExitVehicle(Client player, NetHandle vehicle, int targetSeat)
    {
        API.setPlayerSeatbelt(player, false);

        if (API.hasEntitySyncedData(player, "vehconcetest"))
        {
            EnviarMensagemSubtitulo(player, "Você saiu do veículo do test-drive e ele foi levado de volta para a concessionária.");
            var veh = (NetHandle)API.getEntitySyncedData(player, "vehconcetest");
            API.deleteEntity(veh);
            API.resetEntitySyncedData(player, "vehconcetest");
        }

        if (API.hasEntitySyncedData(player.handle, "player_abastecendo"))
        {
            EnviarMensagemErro(player, "Você parou o abastecimento");
            API.resetEntitySyncedData(player.handle, "player_abastecendo");
        }

        if (API.hasEntitySyncedData(player, "pedconce"))
        {
            var ped = (NetHandle)API.getEntitySyncedData(player, "pedconce");
            API.deleteEntity(ped);
            API.resetEntitySyncedData(player, "pedconce");
        }

        if (API.hasEntitySyncedData(player, "AutoEscola_Teste"))
        {
            if (API.getEntitySyncedData(player.handle, "AutoEscola_Teste") > 0)//Auto Escola
            {
                API.sendNotificationToPlayer(player, "~r~Você tem 2 minutos para voltar ao veículo da auto escola.");

                API.delay(120000, true, () =>
                {
                    if (API.getPlayerVehicle(player) != API.getEntitySyncedData(player.handle, "CarJob"))
                    {
                        NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
                        API.deleteEntity(CarDel);
                        API.setEntitySyncedData(player.handle, "CarJob", null);

                        var pedcc = (NetHandle)API.getEntitySyncedData(player, "pedautoescola");
                        API.deleteEntity(pedcc);
                        API.resetEntitySyncedData(player, "pedautoescola");

                        API.setEntitySyncedData(player.handle, "AutoEscola_Teste", 0);
                        API.setEntitySyncedData(player, "VelMax", 0);

                        API.sendNotificationToPlayer(player, "~r~Teste da auto escola cancelado.");
                    }
                });

            }
        }

        if (API.hasEntitySyncedData(player.handle, "Rota") && GetPlayerJob(player) == 2)
        {
            if (API.getEntitySyncedData(player.handle, "Rota") != 0)
            {
                API.sendNotificationToPlayer(player, "~r~Você tem 2 minutos para voltar ao seu ônibus.");

                API.delay(120000, true, () =>
                {
                    if (API.getPlayerVehicle(player) != API.getEntitySyncedData(player.handle, "CarJob"))
                    {
                        parando_rota(player);
                    }
                });
            }
        }


        if (vehicle == null) return;

        var vehh = recuperarVeiculo(vehicle);

        if (vehh == null) return;

        if (vehh.Modelo == "Taxi") Taxi_OnPlayerExitVehicleHandler(player, vehicle);

    }

    private void API_onVehicleDeath(NetHandle vehicle)
    {
        var veh = recuperarVeiculo(vehicle);

        //Se o dono estiver onlie, ele recebe uma notificação.
        if (GetPlayerFromSQLID(veh.IDPersonagemProprietario) != null)
        {
            API.sendPictureNotificationToPlayer(GetPlayerFromSQLID(veh.IDPersonagemProprietario), "O seu " + veh.Modelo + " explodiu..\nVocê pode passar em nossa loja para restaura-lo.", "CHAR_MP_MORS_MUTUAL", 0, 0, "Mors Mutual Seguros", "Sinistro");
        }

        API.delay(15000, true, () =>
        {
            //var cor1 = API.getVehiclePrimaryColor(vehicle);
            //var cor2 = API.getVehicleSecondaryColor(vehicle);
            //var model = API.getEntityModel(vehicle);

            API.deleteEntity(vehicle);
            veh.Explodido = 1;
            /*
            veh.Veh = API.createVehicle((VehicleHash)model, veh.Posicao, veh.Rotacao, cor1, cor2);
            veh.Veh.engineStatus = false;
            API.setVehicleLocked(veh.Veh, true);
            */



            var str = string.Format("UPDATE veiculos SET explodido=1 WHERE ID={0}", veh.ID);
            var cmd = new MySqlCommand(str, bancodados);
            cmd.ExecuteNonQuery();

            //veh2.Veh.delete();
            veiculos.Remove(veh);

            //veiculos[veiculos.IndexOf(veh)].Veh = veh.Veh;
        });
    }
    #endregion

    public void SetarPlayerFerido(Client player)
    {
        API.setEntitySyncedData(player, "posmorte", player.position);
        API.setEntitySyncedData(player, "ferido", true);

        API.setEntitySyncedData(player, "CAIDO_MORTO", 1);
        API.setPlayerHealth(player, 5);
        API.sendNotificationToPlayer(player, "~r~Você foi gravemente ferido! DEBUG 2");
        API.sendNotificationToPlayer(player, "Você poderá usar o comando /aceitarmorte em 3 minutos. DEBUG 2");
        //API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "dead", "dead_d");
        //API.freezePlayer(player, true);
        API.setEntityInvincible(player, true);
        API.triggerClientEvent(player, "player_ragdoll", 0);

        API.delay(TEMPO_ACEITAR_MORTE, true, () =>
        {
            if (API.getEntitySyncedData(player, "ferido"))
            {
                API.setEntitySyncedData(player, "morto", true);
                API.sendNotificationToPlayer(player, "Você pode usar o comando /aceitarmorte.");
            }
        });
    }

    #region Client Triggers
    public void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
    {
        switch (eventName)
        {
            case "PlayerAsDead":
                SetarPlayerFerido(player);
                break;
            case "Player_Pressionou_Y":

                if (API.isPlayerInAnyVehicle(player))
                {
                    if (API.getEntitySyncedData(player.handle, "player_abastecendo") == 1)
                    {
                        API.resetEntitySyncedData(player.handle, "player_abastecendo");
                        EnviarMensagemErro(player, "Você parou o abastecimento");
                    }
                    else
                    {
                        if (PlayerPertoPosto(player) <= 8f)
                            API.setEntitySyncedData(player.handle, "player_abastecendo", 1);
                        else
                            EnviarMensagemErro(player, "O posto mais próximo está a " + PlayerPertoPosto(player) + " metros");
                    }
                }

                break;
            case "set_dogpos":
                if (isInt(arguments[0].ToString()) == 1)
                    API.setEntitySyncedData(player, "DogPos", arguments[1]);
                else
                    API.setEntitySyncedData(player, "DogMeAtacandoPos", arguments[1]);
                break;
            case "help_page":
                var lista = comandos.Where(c => API.getEntitySyncedData(player, "staff") >= c.Staff).ToList();
                API.triggerClientEvent(player, "player_help", API.toJson(lista));
                break;
            case "stats_page":
                var str_stats = string.Format("{0}|{1}|{2}|{3}|{4}",
                    player.name,
                    player.health,
                    player.armor,
                    API.getEntitySyncedData(player, "dinheiro"),
                    API.getEntitySyncedData(player, "banco"));

                API.triggerClientEvent(player, "player_stats", str_stats);
                break;
            case "tab_players":
                var list = new List<Dictionary<string, object>>();
                foreach (var ply in API.getAllPlayers())
                {
                    if (!API.hasEntitySyncedData(ply, "logado")) continue;

                    var dic = new Dictionary<string, object>();
                    dic["id"] = recuperarIDPorClient(ply);
                    dic["nome"] = ply.name.Replace("_", " ");
                    dic["level"] = API.getEntitySyncedData(ply, "level");
                    dic["ping"] = ply.ping;
                    list.Add(dic);
                }
                API.triggerClientEvent(player, "tab_players", API.toJson(list), list.Count.ToString());
                break;
            case "acaoatm":
                AcaoATM(player, arguments[0].ToString(), isInt(arguments[1].ToString()));
                break;
            case "stopanim":
                if (!ChecarPlayerAnim(player))
                    return;
                if (API.getEntitySyncedData(player.handle, "NaoPodePararAnim") == 1)
                    return;

                API.stopPlayerAnimation(player);
                break;
            case "trancar_destrancar_v":
                InteracaoAbrirTrancar(player);
                break;
            case "ligar_veiculo":
                MotorVeiculo(player, 1);
                break;
            case "desligar_veiculo":
                MotorVeiculo(player, 2);
                break;
            case "autenticarusuario":
                AutenticarUsuario(player, arguments[0].ToString(), arguments[1].ToString());
                break;
            case "kickuser":
                API.kickPlayer(player);
                break;
            case "elm":
                if (player.isInVehicle && player.vehicleSeat == -1)
                {
                    if (API.getVehicleSirenState(player.vehicle))
                    {
                        if (API.hasEntitySyncedData(player.vehicle, "elm"))
                        {
                            API.resetEntitySyncedData(player.vehicle, "elm");
                            API.sendNativeToAllPlayers(0xD8050E0EB60CF274, player.vehicle, false);
                        }
                        else
                        {
                            API.sendNativeToAllPlayers(0xD8050E0EB60CF274, player.vehicle, true);
                            API.setEntitySyncedData(player.vehicle, "elm", true);
                        }
                    }
                }
                break;
            case "respostamenu":
                if (arguments[0].ToString().Contains("ARMÁRIO"))
                {
                    var armario = isInt(arguments[0].ToString().Split(' ')[1].Trim());
                    PegarItemArmario(player, armario, arguments[1].ToString());
                }
                else if (arguments[0].ToString().Equals("INTERAÇÕES"))
                {
                    switch (arguments[1].ToString())
                    {
                        case "Entrar":
                            InteracaoEntrar(player);
                            break;
                        case "Sair":
                            InteracaoSair(player);
                            break;
                        case "Motor":
                            MotorVeiculo(player);
                            break;
                        case "Abrir / Trancar":
                            InteracaoAbrirTrancar(player);
                            break;
                        case "Porta-Malas":
                            InteracaoAbrirTrancarPortaMalas(player);
                            break;
                        case "Capô":
                            InteracaoAbrirTrancarCapo(player);
                            break;
                        case "Porta Dianteira Esquerda":
                            InteracaoAbrirTrancarPortaVeh(player, 0);
                            break;
                        case "Porta Dianteira Direita":
                            InteracaoAbrirTrancarPortaVeh(player, 1);
                            break;
                        case "Porta Traseira Esquerda":
                            InteracaoAbrirTrancarPortaVeh(player, 2);
                            break;
                        case "Porta Traseira Direita":
                            InteracaoAbrirTrancarPortaVeh(player, 3);
                            break;
                    }
                }
                else if (arguments[0].ToString().Equals("PROPRIEDADES"))
                {
                    EntrarPropriedade(player, isInt(arguments[1].ToString().Split(',')[0]), 1);
                }
                else if (arguments[0].ToString().Equals("COMPRAR CASA"))
                {
                    if (arguments[1].ToString() == "Fechar menu") return;
                    else
                    {
                        var propLoop = recuperarPropriedadePorID(API.getEntitySyncedData(player, "Prop_Menu"));
                        API.setEntitySyncedData(player, "Prop_Menu", 0);
                        if (propLoop.IDPersonagemProprietario != 0)
                        {
                            EnviarMensagemErro(player, "Essa propriedade não está a venda.");
                            return;
                        }
                        var index = propriedades.IndexOf(propLoop);

                        var str = string.Empty;
                        str = string.Format(@"SELECT Count(ID) ID FROM propriedades WHERE IDPersonagemProprietario = {0}", API.getEntitySyncedData(player, "id"));
                        var cmd = new MySqlCommand(str, bancodados);
                        var dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            if (isInt(dr["ID"].ToString()) >= MAX_PERSONAGENS_PROPRIEDADES)
                            {
                                EnviarMensagemErro(player, "Você atingiu o limite de propriedades por personagem.");
                                dr.Close();
                                return;
                            }
                        }
                        dr.Close();

                        if (API.getEntitySyncedData(player, "dinheiro") < propLoop.ValorPropriedade)
                        {
                            EnviarMensagemErro(player, "Dinheiro insuficiente.");
                            return;
                        }
                        API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - propLoop.ValorPropriedade);
                        EnviarMensagemSucesso(player, "Propriedade adquirida.");
                        var strUpdate = string.Format("UPDATE propriedades SET IDPersonagemProprietario = {0} WHERE ID = {1}", API.getEntitySyncedData(player, "id"), propLoop.ID);
                        cmd = new MySqlCommand(strUpdate, bancodados);
                        cmd.ExecuteNonQuery();

                        propriedades[index].NomePersonagemProprietario = player.name;

                        RecarregarPropriedade(player, propLoop.ID, "proprietario", string.Format("{0}", API.getEntitySyncedData(player, "id")));
                        return;
                    }
                }
                else if (arguments[0].ToString().Equals("APARTAMENTO"))
                {
                    if (arguments[1].ToString() == "Fechar menu") return;
                    else if (arguments[1].ToString() == "Entrar")
                    {
                        EntrarPropriedade(player, API.getEntitySyncedData(player, "Prop_Menu"), 0);
                        API.setEntitySyncedData(player, "Prop_Menu", 0);
                    }
                    else if (arguments[1].ToString() == "Trancar" || arguments[1].ToString() == "Destrancar")
                    {
                        var propLoop = recuperarPropriedadePorID(API.getEntitySyncedData(player, "Prop_Menu"));
                        API.setEntitySyncedData(player, "Prop_Menu", 0);
                        if (propLoop.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                        {
                            EnviarMensagemErro(player, "Você não tem as chaves desta porta.");
                            return;
                        }

                        var index = propriedades.IndexOf(propLoop);
                        propriedades[index].StatusPortaFrente = !propriedades[index].StatusPortaFrente;
                        if (propriedades[index].StatusPortaFrente)
                            EnviarMensagemSucesso(player, "Você abriu a porta da frente.");
                        else
                            API.sendNotificationToPlayer(player, "~r~Você trancou a porta da frente.");
                        return;
                    }
                    else if (arguments[1].ToString() == "Abandonar")
                    {
                        var param1 = new List<string>();
                        var param2 = new List<string>();

                        API.sendNotificationToPlayer(player, "Você não irá receber nenhum dinheiro de volta ao abandonar a casa.");

                        param1.Add(string.Format("Abandonar"));
                        param2.Add(string.Format("Você tem certeza?"));

                        param1.Add(string.Format("Cancelar"));
                        param2.Add(string.Format("Continuar com a propriedade."));

                        API.triggerClientEvent(player, "menucomresposta", "ABANDONAR", "Listagem de Propriedades", 6, param1, param2);
                    }

                }
                else if (arguments[0].ToString().Equals("ABANDONAR"))
                {
                    if (arguments[1].ToString() == "Cancelar") return;
                    else
                    {
                        var prop = recuperarPropriedadePorID(API.getEntitySyncedData(player, "Prop_Menu"));
                        API.setEntitySyncedData(player, "Prop_Menu", 0);

                        if (prop.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                        {
                            EnviarMensagemErro(player, "Você não é o proprietário dessa propriedade.");
                            return;
                        }

                        EnviarMensagemSucesso(player, "Você abandonou a propriedade.");
                        var str = string.Empty;
                        str = string.Format("UPDATE propriedades SET IDPersonagemProprietario = 0 WHERE ID = {0}", prop.ID);
                        var cmd2 = new MySqlCommand(str, bancodados);
                        cmd2.ExecuteNonQuery();

                        RecarregarPropriedade(player, prop.ID, "proprietario", "0");
                    }
                }
                else if (arguments[0].ToString().Equals("UNIFORMES"))
                {
                    var weaponData = new Dictionary<WeaponHash, CWeaponData>();
                    foreach (WeaponHash wepHash in API.getPlayerWeapons(player))
                        weaponData.Add(wepHash,
                            new CWeaponData
                            {
                                Ammo = API.getPlayerWeaponAmmo(player, wepHash),
                                Tint = API.getPlayerWeaponTint(player, wepHash),
                                Components = JsonConvert.SerializeObject(API.getPlayerWeaponComponents(player, wepHash))
                            });

                    if (arguments[1].ToString().Equals("Civil"))
                    {
                        if (API.getEntitySyncedData(player, "skin2") == "")
                        {
                            EnviarMensagemErro(player, "Você não possui uma skin civil memorizada.");
                            return;
                        }

                        API.setPlayerSkin(player, API.pedNameToModel(API.getEntitySyncedData(player, "skin2")));
                        var clothes = (List<object>)API.getEntitySyncedData(player, "clothes");
                        foreach (var c in clothes)
                            API.setPlayerClothes(player, isInt(c.ToString().Split('|')[0]), isInt(c.ToString().Split('|')[1]), isInt(c.ToString().Split('|')[2]));

                        var accessorys = (List<object>)API.getEntitySyncedData(player, "accessorys");
                        foreach (var acc in accessorys)
                            API.setPlayerAccessory(player, isInt(acc.ToString().Split('|')[0]), isInt(acc.ToString().Split('|')[1]), isInt(acc.ToString().Split('|')[2]));

                        API.setEntitySyncedData(player, "skin2", string.Empty);
                        API.setEntitySyncedData(player, "clothes", new List<string>());
                        API.setEntitySyncedData(player, "accessorys", new List<string>());
                    }
                    else
                    {
                        // Salva a skin civil se for diferente das policiais / bombeiros
                        var skinAtual = ((PedHash)player.model).ToString().ToLower();
                        if (!(skinAtual == "cop01smy" || skinAtual == "cop01sfy" || skinAtual == "swat01smy" || skinAtual == "hwaycop01smy" || skinAtual == "fireman01smy" || skinAtual == "paramedic01smm"))
                        {
                            API.setEntitySyncedData(player, "skin2", ((PedHash)player.model).ToString());

                            var clothes = new List<string>();
                            for (var i = 0; i <= 11; i++)
                            {
                                if (API.getPlayerClothesDrawable(player, i) == 0) continue;

                                clothes.Add(string.Format("{0}|{1}|{2}", i, API.getPlayerClothesDrawable(player, i), API.getPlayerClothesTexture(player, i)));
                            }

                            API.setEntitySyncedData(player, "clothes", clothes);
                        }

                        API.setPlayerSkin(player, API.pedNameToModel(arguments[1].ToString()));

                        if (arguments[1].ToString() != "SWAT01SMY")
                            API.triggerClientEvent(player, "uniformes", arguments[1].ToString());
                    }

                    foreach (var weapon in weaponData)
                    {
                        API.givePlayerWeapon(player, weapon.Key, weapon.Value.Ammo, false, true);
                        API.setPlayerWeaponTint(player, weapon.Key, weapon.Value.Tint);

                        var weaponMods = JsonConvert.DeserializeObject<List<WeaponComponent>>(weapon.Value.Components);
                        foreach (WeaponComponent compID in weaponMods) API.givePlayerWeaponComponent(player, weapon.Key, compID);
                    }
                }
                break;
            case "alteraracessorio":
                API.setPlayerAccessory(player, isInt(arguments[0].ToString()), isInt(arguments[1].ToString()), isInt(arguments[2].ToString()));
                break;
            case "limparacessorio":
                API.clearPlayerAccessory(player, isInt(arguments[0].ToString()));
                break;
            case "alterarvariacao":
                API.setPlayerClothes(player, isInt(arguments[0].ToString()), isInt(arguments[1].ToString()), isInt(arguments[2].ToString()));
                break;
            case "confirmarvariacoes":
                ConfirmarVariacoes(player);
                break;
            case "resetarvariacoes":
                ResetarVariacoes(player);
                break;
            case "alterarpersonagem":
                if (API.hasEntitySyncedData(player, "pedlogin"))
                {
                    var ped = (NetHandle)API.getEntitySyncedData(player, "pedlogin");
                    var descs = API.getEntitySyncedData(player, "pedinfo");
                    var descs_id = API.getEntitySyncedData(player, "pedinfo_id");
                    API.deleteEntity(ped);
                    ped = API.createPed(API.pedNameToModel(descs[isInt(arguments[0].ToString())]), new Vector3(402.9244, -996.288, -99.00025), 180, API.getEntitySyncedData(player, "dimensaorandom"));
                    API.setEntitySyncedData(player, "pedlogin", ped);

                    if (descs[isInt(arguments[0].ToString())] == "FreemodeMale01" || descs[isInt(arguments[0].ToString())] == "FreemodeFemale01")
                    {
                        var string_loadp = string.Format("SELECT * FROM personagem_custom WHERE PersID = {0} LIMIT 1", descs_id[isInt(arguments[0].ToString())]);
                        var cmd_loadp = new MySqlCommand(string_loadp, bancodados);
                        var lendoPers = cmd_loadp.ExecuteReader();

                        if (lendoPers.HasRows)
                        {
                            lendoPers.Read();
                            API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 3, isInt(lendoPers["Clothes_3"].ToString()), isInt(lendoPers["Clothes_3_T"].ToString()), 2);
                            API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 4, isInt(lendoPers["Clothes_4"].ToString()), isInt(lendoPers["Clothes_4_T"].ToString()), 2);
                            API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 6, isInt(lendoPers["Clothes_6"].ToString()), isInt(lendoPers["Clothes_6_T"].ToString()), 2);
                            API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 8, isInt(lendoPers["Clothes_8"].ToString()), isInt(lendoPers["Clothes_8_T"].ToString()), 2);
                            API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 11, isInt(lendoPers["Clothes_11"].ToString()), isInt(lendoPers["Clothes_11_T"].ToString()), 2);

                            API.sendNativeToPlayer(player, Hash.SET_PED_HEAD_BLEND_DATA, ped,
                                             isInt(lendoPers["GTAO_SHAPE_FIRST_ID"].ToString()), isInt(lendoPers["GTAO_SHAPE_SECOND_ID"].ToString()), 0,
                                             isInt(lendoPers["GTAO_SKIN_FIRST_ID"].ToString()), isInt(lendoPers["GTAO_SKIN_SECOND_ID"].ToString()), 0,
                                             isFloat(lendoPers["GTAO_SHAPE_MIX"].ToString()), isFloat(lendoPers["GTAO_SKIN_MIX"].ToString()), 0, false);

                            API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 2, isInt(lendoPers["GTAO_HAIR_STYLE"].ToString()), 1);
                            API.sendNativeToPlayer(player, Hash._SET_PED_HAIR_COLOR, ped, isInt(lendoPers["GTAO_HAIR_COLOR"].ToString()), isInt(lendoPers["GTAO_HAIR_HIGHLIGHT_COLOR"].ToString()));

                            API.sendNativeToPlayer(player, Hash._SET_PED_EYE_COLOR, ped, isInt(lendoPers["GTAO_EYE_COLOR"].ToString()));

                            API.sendNativeToPlayer(player, Hash.SET_PED_HEAD_OVERLAY, ped, 2, isInt(lendoPers["GTAO_EYEBROWS"].ToString()), 1f);
                            API.sendNativeToPlayer(player, Hash._SET_PED_HEAD_OVERLAY_COLOR, ped, 2, 1, isInt(lendoPers["GTAO_EYEBROWS_COLOR"].ToString()), isInt(lendoPers["GTAO_EYEBROWS_COLOR2"].ToString()));

                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 0, isInt(lendoPers["GTAO_FACE_FEATURES_1"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 1, isInt(lendoPers["GTAO_FACE_FEATURES_2"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 2, isInt(lendoPers["GTAO_FACE_FEATURES_3"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 3, isInt(lendoPers["GTAO_FACE_FEATURES_4"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 4, isInt(lendoPers["GTAO_FACE_FEATURES_5"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 5, isInt(lendoPers["GTAO_FACE_FEATURES_6"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 6, isInt(lendoPers["GTAO_FACE_FEATURES_7"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 7, isInt(lendoPers["GTAO_FACE_FEATURES_8"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 8, isInt(lendoPers["GTAO_FACE_FEATURES_9"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 9, isInt(lendoPers["GTAO_FACE_FEATURES_10"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 10, isInt(lendoPers["GTAO_FACE_FEATURES_11"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 11, isInt(lendoPers["GTAO_FACE_FEATURES_12"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 12, isInt(lendoPers["GTAO_FACE_FEATURES_13"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 13, isInt(lendoPers["GTAO_FACE_FEATURES_14"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 14, isInt(lendoPers["GTAO_FACE_FEATURES_15"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 15, isInt(lendoPers["GTAO_FACE_FEATURES_16"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 16, isInt(lendoPers["GTAO_FACE_FEATURES_17"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 17, isInt(lendoPers["GTAO_FACE_FEATURES_18"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 18, isInt(lendoPers["GTAO_FACE_FEATURES_19"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 19, isInt(lendoPers["GTAO_FACE_FEATURES_20"].ToString()));
                            API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 20, isInt(lendoPers["GTAO_FACE_FEATURES_21"].ToString()));
                        }
                        lendoPers.Close();

                    }

                }
                break;
            case "confirmarpersonagem":
                CarregarPersonagem(player, arguments[0].ToString().Replace(" ", "_"));
                break;
            case "visualizarveiculo":
                VisualizarVeiculoConce(player, arguments[0].ToString());
                break;
            case "sairconce":
                SairConce(player);
                break;
            case "acaoveiculoconce":
                AcaoVeiculoConce(player, arguments[0].ToString(), isInt(arguments[1].ToString()), isInt(arguments[2].ToString()), isInt(arguments[3].ToString()));
                break;
            case "IniciarTesteAutoEscola":
                switch (isInt(arguments[0].ToString()))
                {
                    case 1: //Carro
                        Vehicle jobCar = API.createVehicle(VehicleHash.Blista, new Vector3(118.8341, -1698.233, 28.97441), new Vector3(0.3626092, -1.767728, 140.703), 0, 0);

                        API.setVehicleCustomPrimaryColor(jobCar, 255, 255, 255);
                        API.setVehicleCustomSecondaryColor(jobCar, 255, 255, 255);
                        API.setVehicleEngineStatus(jobCar, false);
                        API.setVehicleNumberPlateStyle(jobCar, 2);
                        string placa = "ESTUDANTE";
                        API.setVehicleNumberPlate(jobCar, placa);

                        API.setPlayerIntoVehicle(player, jobCar, -1);
                        API.setEntitySyncedData(player.handle, "CarJob", jobCar);
                        API.setEntitySyncedData(player.handle, "Parte", 1);
                        API.setEntitySyncedData(player.handle, "AutoEscola_Teste", 1);
                        API.setEntitySyncedData(player, "VelMax", 0);

                        Ped instrutor = API.createPed(PedHash.Andreas, new Vector3(), 0);
                        API.setEntityPosition(instrutor, new Vector3(117.5829, -1696.912, 29.23483));
                        API.setEntityPositionFrozen(instrutor, false);
                        API.setEntityInvincible(instrutor, false);

                        API.sendNativeToAllPlayers(Hash.TASK_ENTER_VEHICLE, instrutor, jobCar, -1, 0, 1.0, 1, 0);

                        API.setEntitySyncedData(player, "pedautoescola", instrutor);

                        API.delay(2000, true, () =>
                        {
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Olá, vamos começar o teste, pegue a direita.");
                            API.setVehicleEngineStatus(jobCar, true);
                        });

                        API.triggerClientEvent(player, "bus_checkpoint", GetNextPoint_AutoEscola(player, 0), 0);
                        break;
                    case 2: //Caminhão
                        break;
                }
                break;
            /* Personagem Custom */
            case "comprou_roupa":

                byte tipo = Convert.ToByte(arguments[0]);
                byte modelo = Convert.ToByte(arguments[1]);
                byte comprando = Convert.ToByte(arguments[2]);
                byte textura = Convert.ToByte(arguments[3]);

                switch (tipo)
                {
                    case 1:
                        Comprando_Calca(player, modelo, textura, comprando);
                        break;
                    case 2:
                        Comprando_Bermuda(player, modelo, textura, comprando);
                        break;
                    case 3:
                        Comprando_Camisa(player, modelo, textura, comprando);
                        break;
                    case 4:
                        Comprando_CasacoMoletom(player, modelo, textura, comprando);
                        break;
                    case 5:
                        Comprando_Calcado(player, modelo, textura, comprando);
                        break;
                }
                break;
            case "Reverter_roupa":
                SetarMinhasRoupas(player);
                break;
            case "Criar_Pers_H":
                PedHash skinCustom = (PedHash)1885233650;
                API.setPlayerSkin(player, skinCustom);

                TrocaClotheSlot(player, 11, 1, 0, 1);
                TrocaClotheSlot(player, 8, 15, 0, 1);
                TrocaClotheSlot(player, 3, 0, 0, 1);

                initializePedFace(player, 1);
                updatePlayerFace(player);
                break;

            case "Criar_Pers_M":
                skinCustom = API.pedNameToModel("freemodefemale01"); ;
                API.setPlayerSkin(player, skinCustom);

                TrocaClotheSlot(player, 11, 1, 0, 1);
                TrocaClotheSlot(player, 8, 15, 0, 1);
                TrocaClotheSlot(player, 3, 0, 0, 1);

                initializePedFace(player, 1);
                updatePlayerFace(player);
                break;

            case "Criando_Pers_Update":
                switch (isInt(arguments[0].ToString()))
                {
                    case 0: API.setEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID", isInt(arguments[1].ToString())); break;
                    case 1: API.setEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID", isInt(arguments[1].ToString())); break;
                    case 2: API.setEntitySyncedData(player, "GTAO_SKIN_FIRST_ID", isInt(arguments[1].ToString())); break;
                    case 3: API.setEntitySyncedData(player, "GTAO_SKIN_SECOND_ID", isInt(arguments[1].ToString())); break;
                    case 4: API.setEntitySyncedData(player, "GTAO_SHAPE_MIX", isFloat(arguments[1].ToString())); break;
                    case 5: API.setEntitySyncedData(player, "GTAO_SKIN_MIX", isFloat(arguments[1].ToString())); break;
                    case 6: API.setEntitySyncedData(player, "GTAO_HAIR_COLOR", isInt(arguments[1].ToString())); break;
                    case 7: API.setEntitySyncedData(player, "GTAO_HAIR_HIGHLIGHT_COLOR", isInt(arguments[1].ToString())); break;
                    case 8: API.setEntitySyncedData(player, "GTAO_EYE_COLOR", isInt(arguments[1].ToString())); break;
                    case 9: API.setEntitySyncedData(player, "GTAO_EYEBROWS_COLOR", isInt(arguments[1].ToString())); break;
                    case 10: API.setEntitySyncedData(player, "GTAO_MAKEUP_COLOR", isInt(arguments[1].ToString())); break;
                    case 11: API.setEntitySyncedData(player, "GTAO_LIPSTICK_COLOR", isInt(arguments[1].ToString())); break;
                    case 12: API.setEntitySyncedData(player, "GTAO_EYEBROWS_COLOR2", isInt(arguments[1].ToString())); break;
                    case 13: API.setEntitySyncedData(player, "GTAO_MAKEUP_COLOR2", isInt(arguments[1].ToString())); break;
                    case 14: API.setEntitySyncedData(player, "GTAO_LIPSTICK_COLOR2", isInt(arguments[1].ToString())); break;
                    case 15: API.setEntitySyncedData(player, "GTAO_HAIR_STYLE", isInt(arguments[1].ToString())); break;
                    case 16: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_1", isFloat(arguments[1].ToString())); break;
                    case 17: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_2", isFloat(arguments[1].ToString())); break;
                    case 18: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_3", isFloat(arguments[1].ToString())); break;
                    case 19: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_4", isFloat(arguments[1].ToString())); break;
                    case 20: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_5", isFloat(arguments[1].ToString())); break;
                    case 21: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_6", isFloat(arguments[1].ToString())); break;
                    case 22: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_7", isFloat(arguments[1].ToString())); break;
                    case 23: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_8", isFloat(arguments[1].ToString())); break;
                    case 24: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_9", isFloat(arguments[1].ToString())); break;
                    case 25: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_10", isFloat(arguments[1].ToString())); break;
                    case 26: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_11", isFloat(arguments[1].ToString())); break;
                    case 27: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_12", isFloat(arguments[1].ToString())); break;
                    case 28: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_13", isFloat(arguments[1].ToString())); break;
                    case 29: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_14", isFloat(arguments[1].ToString())); break;
                    case 30: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_15", isFloat(arguments[1].ToString())); break;
                    case 31: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_16", isFloat(arguments[1].ToString())); break;
                    case 32: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_17", isFloat(arguments[1].ToString())); break;
                    case 33: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_18", isFloat(arguments[1].ToString())); break;
                    case 34: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_19", isFloat(arguments[1].ToString())); break;
                    case 35: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_20", isFloat(arguments[1].ToString())); break;
                    case 36: API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_21", isFloat(arguments[1].ToString())); break;
                    case 37: API.setEntitySyncedData(player, "GTAO_EYEBROWS", isInt(arguments[1].ToString())); break;
                }

                updatePlayerFace(player);
                break;

            case "CriouPersonagem":
                API.setEntityPosition(player, new Vector3(402.0585, -713.2081, 28.20688));
                API.freezePlayer(player, false);
                API.triggerClientEvent(player, "VoltarCamera");

                SaveCustomPlayer(player);
                break;
            case "PressedY_Checkpoint":
                byte interacao_id = Convert.ToByte(arguments[0]);
                switch (interacao_id)
                {
                    case 1: //Auto Escola
                        break;
                }
                break;
            case "PressionouEmprego":
                byte jobnum = Convert.ToByte(arguments[0]);
                if (jobnum == 1)
                {
                    if (GetPlayerJob(player) == 0)
                    {
                        API.setEntitySyncedData(player, "Job", 1);
                        API.sendNotificationToPlayer(player, "Parabens pelo seu novo emprego, você agora é um taxista.");
                    }
                    else if (GetPlayerJob(player) == 1)
                        API.sendNotificationToPlayer(player, "~r~Você já é um taxista.");
                    else
                    {
                        API.sendNotificationToPlayer(player, "~b~Você deve sair do seu atual emprego antes de se tornar um taxista.");
                    }
                }
                else if (jobnum == 2)
                {
                    if (GetPlayerJob(player) == 0)
                    {
                        API.setEntitySyncedData(player, "Job", 2);
                        API.sendNotificationToPlayer(player, "Parabens pelo seu novo emprego, você agora é um motorista de onibus.");
                    }
                    else if (GetPlayerJob(player) == 2)
                    {
                        if (API.hasEntitySyncedData(player.handle, "Rota"))
                        {
                            if (API.getEntitySyncedData(player.handle, "Rota") != 0)
                            {
                                API.sendNotificationToPlayer(player, "~r~Você já está em uma rota, use /pararrota para cancela-la.");
                                return;
                            }
                        }

                        var random = new Random();
                        int rota = random.Next(3) + 1;
                        API.sendNotificationToPlayer(player, "~r~Rota: " + rota);

                        API.sendPictureNotificationToPlayer(player, "~s~Començando uma nova rota.~n~~b~Utilize /pararrota caso queira cancelar a rota.", "CHAR_LS_TOURIST_BOARD", 5, 2, "Dashound Bus Center", "Nova rota");
                        Vehicle jobCar = API.createVehicle(VehicleHash.Bus, new Vector3(421.4999, -656.7313, 28.60785), new Vector3(2.745476, 0.5108438, 179.1604), 0, 0);
                        API.setPlayerIntoVehicle(player, jobCar, -1);

                        API.setEntitySyncedData(player.handle, "Rota", rota);
                        API.setEntitySyncedData(player.handle, "Parte", 1);
                        API.setEntitySyncedData(player.handle, "CarJob", jobCar);

                        API.triggerClientEvent(player, "update_bus_job", true, 0, GetTamanhoRota(rota));
                        API.triggerClientEvent(player, "bus_checkpoint", GetNextPoint(player, rota, 0), 0);
                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~b~Você deve sair do seu atual emprego antes de se tornar ou interagir com o emprego de motorista de onibus.");
                    }
                }
                else if (jobnum == 3)
                {
                    if (GetPlayerJob(player) == 0)
                    {
                        API.setEntitySyncedData(player, "Job", 3);
                        API.sendNotificationToPlayer(player, "Parabens pelo seu novo emprego, agora você trabalha na Los Santos Services.");
                    }
                    else if (GetPlayerJob(player) == 3)
                    {
                        if (API.hasEntitySyncedData(player.handle, "Rota"))
                        {
                            if (API.getEntitySyncedData(player.handle, "Rota") != 0)
                            {
                                API.sendNotificationToPlayer(player, "~r~Você já está em um serviço, use /pararservico para cancela-lo.");
                                return;
                            }
                        }


                        var random = new Random();
                        int rota = random.Next(3) + 1;
                        API.sendNotificationToPlayer(player, "~r~Rota: " + rota);

                        API.sendPictureNotificationToPlayer(player, "~s~Començando um novo serviço.~n~~b~Utilize /pararservico caso queira cancelar.", "CHAR_LS_TOURIST_BOARD", 5, 2, "Los Santos Services", "Novo serviço");
                        Vehicle jobCar = API.createVehicle(VehicleHash.UtilliTruck3, new Vector3(-316.5967, -1537.807, 27.31574), new Vector3(2.745476, 0.5108438, 179.1604), 0, 0);

                        API.setVehicleCustomPrimaryColor(jobCar, 255, 255, 255);
                        API.setVehicleCustomSecondaryColor(jobCar, 255, 255, 255);

                        API.setEntitySyncedData(player.handle, "Rota", rota);
                        API.setEntitySyncedData(player.handle, "Parte", 1);
                        API.setEntitySyncedData(player.handle, "CarJob", jobCar);

                        API.triggerClientEvent(player, "update_bus_job", true, 0, GetTamanhoRota_LSS(rota));
                        API.triggerClientEvent(player, "bus_checkpoint", GetNextPoint_LSS(player, rota, 0), 0);

                    }
                    else
                    {
                        API.sendNotificationToPlayer(player, "~b~Você deve sair do seu atual emprego antes de se tornar ou interagir com o emprego da LS Services.");
                    }
                }
                break;

                //=================================
        }
    }
    #endregion

    #region CEFs
    private void AcaoATM(Client player, string acao, int valor)
    {
        if (valor <= 0)
        {
            EnviarMensagemErro(player, "Valor não informado.");
            return;
        }

        if (acao.Equals("SACAR"))
        {
            if (valor > API.getEntitySyncedData(player, "banco"))
            {
                EnviarMensagemErro(player, "Você não possui o valor em sua conta bancária.");
                return;
            }

            API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") + valor);
            API.setEntitySyncedData(player, "banco", API.getEntitySyncedData(player, "banco") - valor);
            EnviarMensagemSucesso(player, string.Format("Você sacou ${0}.", valor));
        }
        else if (acao.Equals("DEPOSITAR"))
        {
            if (valor > API.getEntitySyncedData(player, "dinheiro"))
            {
                EnviarMensagemErro(player, "Você não possui o valor consigo.");
                return;
            }

            API.setEntitySyncedData(player, "banco", API.getEntitySyncedData(player, "banco") + valor);
            API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - valor);
            EnviarMensagemSucesso(player, string.Format("Você depositou ${0}.", valor));
        }
    }
    #endregion

    #region Recuperações
    private Propriedade recuperarPropriedadePorID(int id)
    {
        var prop = propriedades.Where(p => p.ID.Equals(id)).ToList();
        if (prop.Count > 0)
            return prop[0];

        return null;
    }

    private Armario recuperarArmarioPorID(int id)
    {
        var arm = armarios.Where(a => a.ID.Equals(id)).ToList();
        if (arm.Count > 0)
            return arm[0];

        return null;
    }

    private Armario.Item recuperarArmarioItemPorArma(int armario, string arma)
    {
        var arm = armarios.Where(a => a.ID.Equals(armario)).ToList();
        if (arm.Count > 0)
        {
            var item = arm[0].Itens.Where(i => i.Arma.Equals(arma)).ToList();
            if (item.Count > 0)
                return item[0];
        }

        return null;
    }

    private Faccao recuperarFaccaoPorID(int id)
    {
        var fac = faccoes.Where(f => f.ID.Equals(id)).ToList();
        if (fac.Count > 0)
            return fac[0];

        return null;
    }

    private Faccao.Rank recuperarFaccaoRankPorID(int faccao, int id)
    {
        var fac = faccoes.Where(f => f.ID.Equals(faccao)).ToList();
        if (fac.Count > 0)
        {
            var item = fac[0].Ranks.Where(i => i.ID.Equals(id)).ToList();
            if (item.Count > 0)
                return item[0];
        }

        return null;
    }

    private adBlip recuperarBlipPorID(int id)
    {
        var blip = blips.Where(a => a.ID.Equals(id)).ToList();
        if (blip.Count > 0)
            return blip[0];

        return null;
    }

    private adPed recuperarPedPorID(int id)
    {
        var ped = peds.Where(a => a.ID.Equals(id)).ToList();
        if (ped.Count > 0)
            return ped[0];

        return null;
    }

    private SOS recuperarSOSPorID(int id)
    {
        var sos = listaSOS.Where(s => s.IDPlayer.Equals(id)).ToList();
        if (sos.Count > 0)
            return sos[0];

        return null;
    }

    private ATM recuperarATMPorId(int id)
    {
        var atm = atms.Where(a => a.ID.Equals(id)).ToList();
        if (atm.Count > 0)
            return atm[0];

        return null;
    }

    private Ponto recuperarPontoPorID(int id)
    {
        var pt = pontos.Where(a => a.ID.Equals(id)).ToList();
        if (pt.Count > 0)
            return pt[0];

        return null;
    }

    private ItensDropados recuperarItemDropadoPorID(int id)
    {
        var pt = itensDropados.Where(a => a.ID.Equals(id)).ToList();
        if (pt.Count > 0)
            return pt[0];

        return null;
    }

    private Veiculo recuperarVeiculo(NetHandle v)
    {
        var veh = veiculos.Where(a => a.Veh.Equals(v)).ToList();
        if (veh.Count > 0)
            return veh[0];

        return null;
    }

    private Veiculo recuperarVeiculoPorID(int id)
    {
        var veh = veiculos.Where(a => a.ID.Equals(id)).ToList();
        if (veh.Count > 0)
            return veh[0];

        return null;
    }

    private int recuperarPinturaArma(string nomePintura)
    {
        var id = 0;
        switch (nomePintura)
        {
            case "Green": id = 1; break;
            case "Gold": id = 2; break;
            case "Pink": id = 3; break;
            case "Army": id = 4; break;
            case "LSPD": id = 5; break;
            case "Orange": id = 6; break;
            case "Platinum": id = 7; break;
        }
        return id;
    }

    private Client GetPlayerFromSQLID(int clientid)
    {
        List<Client> clients = new List<Client>();
        foreach (var player in players)
        {
            if (player == null) continue;

            if (API.getEntitySyncedData(player, "id") == clientid)
            {
                return player;
            }
        }
        return null;
    }

    private Client findPlayer(Client sender, string idOrName, bool pegarUmPlayer = true)
    {
        var id = 0;
        if (int.TryParse(idOrName, out id))
        {
            return recuperarClientPorID(id);
        }

        List<Client> clients = new List<Client>();
        foreach (var player in players)
        {
            if (player == null) continue;

            if (player.name.ToLower().Contains(idOrName.ToLower()))
            {
                if ((player.name.Equals(idOrName, StringComparison.OrdinalIgnoreCase)) && pegarUmPlayer)
                    return player;
                else
                    clients.Add(player);
            }
        }

        if (!clients.Count.Equals(0))
        {
            if (pegarUmPlayer)
            {
                if (clients.Count.Equals(1))
                {
                    return clients[0];
                }
                else
                {
                    EnviarMensagemErro(sender, string.Format("~r~Múltiplos personagens encontrados para a pesquisa: {0}", idOrName));
                    return null;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, string.Format("Personagens encontrados ({0})", idOrName));
                foreach (var cli in clients)
                    API.sendChatMessageToPlayer(sender, string.Format("{0} (ID: {1})", cli.name, players.IndexOf(cli)));
            }
        }
        else if (!pegarUmPlayer)
        {
            EnviarMensagemErro(sender, string.Format("Nenhum personagem encontrado para a pesquisa: {0}", idOrName));
        }

        return null;
    }

    private Client recuperarClientPorID(int id)
    {
        if (id < 0)
            return null;

        if (players[id] == null)
            return null;

        return players[id];
    }

    private int recuperarIDPorClient(Client target)
    {
        return players.IndexOf(target);
    }

    private int recuperarIDLivre()
    {
        foreach (var item in players)
        {
            if (item == null)
                return players.IndexOf(item);
        }
        return -1;
    }

    private string recuperarNomeStaff(int staff)
    {
        var nome = string.Empty;
        switch (staff)
        {
            case 1: nome = "Tester"; break;
            case 2: nome = "Game Admin 1"; break;
            case 3: nome = "Game Admin 2"; break;
            case 4: nome = "Game Admin 3"; break;
            case 5: nome = "Lead Admin"; break;
            case 6: nome = "Head Admin"; break;
            case 7: nome = "Developer"; break;
            case 8: nome = "Management"; break;
        }
        return nome;
    }

    private string recuperarNomeIC(Client player)
    {
        var nome = string.Empty;

        if (!player.nametag.Contains("Mascarado"))
            nome = player.name;
        else
            nome = player.nametag;

        return nome;
    }

    private int recuperarSlotRadio(Client player, int canal)
    {
        var slot = 0;

        if (API.getEntitySyncedData(player, "canalradio3") == canal)
            slot = 3;
        if (API.getEntitySyncedData(player, "canalradio2") == canal)
            slot = 2;
        if (API.getEntitySyncedData(player, "canalradio") == canal)
            slot = 1;

        return slot;
    }

    private Concessionaria recuperarConcePorID(int id)
    {
        var c = concessionarias.Where(a => a.ID.Equals(id)).ToList();
        if (c.Count > 0)
            return c[0];

        return null;
    }

    private Concessionaria.Veiculo recuperarVeiculoConcePorNome(string nome)
    {
        var veh = veiculosConce.Where(a => a.Nome.ToLower().Equals(nome.ToLower())).ToList();
        if (veh.Count > 0)
            return veh[0];

        return null;
    }
    #endregion

    #region Listagem de Comandos
    private void CarregarComandos()
    {
        comandos.Add(new Comando { Categoria = "Interpretação", Nome = "/me", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interpretação", Nome = "/do", Descricao = "" });

        comandos.Add(new Comando { Categoria = "Chat IC", Nome = "/g", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Chat IC", Nome = "/s", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Chat IC", Nome = "/baixo", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Chat IC", Nome = "/cw", Descricao = "" });

        comandos.Add(new Comando { Categoria = "Outros", Nome = "/id", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/q", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/aceitarmorte", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/login", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/limparmeuchat", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/pagar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/admins", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/sos", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/idveh", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/stats", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/mascara", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/tog", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/aceitar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/recusar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/levelup", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/atm", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/r", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/r2", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/r3", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/canal", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Outros", Nome = "/animlist", Descricao = "" });

        comandos.Add(new Comando { Categoria = "Facções", Nome = "/membros", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/blockf", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/rank", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/expulsar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/convidar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/armario", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/m", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/hq", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Facções", Nome = "/gov", Descricao = "" });

        comandos.Add(new Comando { Categoria = "Chat OOC", Nome = "/pm", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Chat OOC", Nome = "/o", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Chat OOC", Nome = "/b", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Chat OOC", Nome = "/f", Descricao = "" });

        comandos.Add(new Comando { Categoria = "Interações", Nome = "/entrar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/sair", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/trancar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/comprar", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/vender", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/portamalas", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/capo", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/porta", Descricao = "" });
        comandos.Add(new Comando { Categoria = "Interações", Nome = "/motor", Descricao = "" });

        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/kick", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/ir", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/trazer", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/aj", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/rj", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/listarsos", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/apm", Descricao = "" });
        comandos.Add(new Comando { Staff = 1, Categoria = "Tester", Nome = "/atrabalho", Descricao = "" });

        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/a", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/reviver", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/checaratirador", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/vida", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/congelar", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/descongelar", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/skin", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/spec", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/unspec", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/teleportar", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/ban", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/bantemp", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/unban", Descricao = "" });
        comandos.Add(new Comando { Staff = 2, Categoria = "Game Admin 1", Nome = "/proximo", Descricao = "" });

        comandos.Add(new Comando { Staff = 3, Categoria = "Game Admin 2", Nome = "/irprop", Descricao = "" });
        comandos.Add(new Comando { Staff = 3, Categoria = "Game Admin 2", Nome = "/irveh", Descricao = "" });
        comandos.Add(new Comando { Staff = 3, Categoria = "Game Admin 2", Nome = "/trazerveh", Descricao = "" });
        comandos.Add(new Comando { Staff = 3, Categoria = "Game Admin 2", Nome = "/resetarinv", Descricao = "" });


        comandos.Add(new Comando { Staff = 5, Categoria = "Lead Admin", Nome = "/colete", Descricao = "" });

        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/criarfac", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/faccoes", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/editarfac", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/lider", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/criarprop", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/editarprop", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/arma", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/municao", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/criarveh", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/editarveh", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/ranks", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/editarrank", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/criararmario", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/editararmario", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/armarios", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/addaitem", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/checararmario", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/irarmario", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/editaraitem", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/excluiraitem", Descricao = "" });
        comandos.Add(new Comando { Staff = 6, Categoria = "Head Admin", Nome = "/excluirarmario", Descricao = "" });

        comandos.Add(new Comando { Staff = 7, Categoria = "Developer", Nome = "/getpos", Descricao = "" });
        comandos.Add(new Comando { Staff = 7, Categoria = "Developer", Nome = "/irpos", Descricao = "" });

        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/staff", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/tempo", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/dinheiro", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/criarblip", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/criaratm", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/criarped", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/excluirblip", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/editarblip", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/irblip", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/excluirped", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/irped", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/editarped", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/excluiratm", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/editaratm", Descricao = "" });
        comandos.Add(new Comando { Staff = 8, Categoria = "Management", Nome = "/iratm", Descricao = "" });

        comandos = comandos.OrderBy(c => c.Categoria).ThenBy(c => c.Nome).ToList();
    }
    #endregion

    #region Concessionárias
    private void CarregarConcessionarias()
    {
        var cmd = new MySqlCommand("SELECT * FROM concessionarias", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var c = new Concessionaria();
            c.ID = isInt(dr["ID"].ToString());
            c.Tipo = dr["Tipo"].ToString();
            c.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            c.Text = API.createTextLabel("[/v comprar]", c.Posicao, 30, (float)0.4);
            c.Posicao_vspawn = new Vector3(isFloat(dr["PosX_v"].ToString()), isFloat(dr["PosY_v"].ToString()), isFloat(dr["PosZ_v"].ToString()));

            concessionarias.Add(c);
        }
        dr.Close();

        cmd = new MySqlCommand("SELECT * FROM concessionariasveiculos", bancodados);
        dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var v = new Concessionaria.Veiculo();
            v.Categoria = dr["Categoria"].ToString();
            v.Nome = dr["Nome"].ToString();
            v.Preco = isInt(dr["Preco"].ToString());
            v.Velocidade = isInt(dr["Velocidade"].ToString());
            v.Frenagem = isInt(dr["Frenagem"].ToString());
            v.Aceleracao = isInt(dr["Aceleracao"].ToString());
            v.Tracao = isInt(dr["Tracao"].ToString());

            veiculosConce.Add(v);
        }
        dr.Close();

        API.consoleOutput("Concessionárias: {0}", concessionarias.Count);
        API.consoleOutput("Veículos para Venda: {0}", veiculosConce.Count);
    }

    private void CriarConce(Client player, string tipo)
    {
        var str = string.Format(@"INSERT INTO concessionarias (PosX, PosZ, PosY, Tipo) VALUES ({0}, {1}, {2}, {3})",
            aFlt(player.position.X),
            aFlt(player.position.Z),
            aFlt(player.position.Y),
            at(tipo)
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var c = new Concessionaria();
        c.ID = id;
        c.Posicao = player.position;
        c.Tipo = tipo;
        c.Text = API.createTextLabel("[/v comprar]", c.Posicao, 30, (float)0.4);

        concessionarias.Add(c);
    }

    private void VisualizarVeiculoConce(Client player, string veiculo)
    {
        var veh = API.vehicleNameToModel(veiculo);
        if (veh == 0) return;

        API.setEntityDimension(player, API.getEntitySyncedData(player, "id"));
        var v = API.createVehicle(veh, new Vector3(-44.1876, -1097.85, 26.0097), new Vector3(-0.087068, 0.0634254, 91), 1, 1, API.getEntitySyncedData(player, "id"));
        API.setEntitySyncedData(player, "vehconce", v);
        API.setEntitySyncedData(player, "nomevehconce", veh.ToString());
        API.triggerClientEvent(player, "criarcamera", new Vector3(-45.49657, -1102.973, 26.42236), new Vector3(-44.1876, -1097.85, 26.0097));
    }

    private void SairConce(Client player)
    {
        if (API.hasEntitySyncedData(player, "vehconce"))
        {
            var veh = (NetHandle)API.getEntitySyncedData(player, "vehconce");
            API.deleteEntity(veh);
            API.resetEntitySyncedData(player, "vehconce");
            API.setEntityDimension(player, 0);
            API.triggerClientEvent(player, "limparcamera");
        }
    }

    private void AcaoVeiculoConce(Client player, string acao, int r = 0, int g = 0, int b = 0)
    {
        if (API.hasEntitySyncedData(player, "vehconce"))
        {
            var veh = (NetHandle)API.getEntitySyncedData(player, "vehconce");
            var nomeVeiculo = API.getEntitySyncedData(player, "nomevehconce");
            var veiculo = veiculosConce.Where(v => v.Nome.ToLower().Equals(nomeVeiculo.ToLower())).ToList()[0];
            var vehc = API.vehicleNameToModel(nomeVeiculo);
            if (vehc == 0) return;
            var vehconce = API.getEntitySyncedData(player, "vehconce");
            var cor1 = API.getVehicleCustomPrimaryColor(vehconce);
            var cor2 = API.getVehicleCustomSecondaryColor(vehconce);

            switch (acao)
            {
                case "portamalas":
                    API.setVehicleDoorState(veh, 5, !API.getVehicleDoorState(veh, 5));
                    break;
                case "capo":
                    API.setVehicleDoorState(veh, 4, !API.getVehicleDoorState(veh, 4));
                    break;
                case "portas":
                    API.setVehicleDoorState(veh, 0, !API.getVehicleDoorState(veh, 0));
                    API.setVehicleDoorState(veh, 1, !API.getVehicleDoorState(veh, 1));
                    API.setVehicleDoorState(veh, 2, !API.getVehicleDoorState(veh, 2));
                    API.setVehicleDoorState(veh, 3, !API.getVehicleDoorState(veh, 3));
                    break;
                case "buzina":
                    API.sendNativeToPlayer(player, Hash.START_VEHICLE_HORN, veh, 2000, "NORMAL", false);
                    break;
                case "testdrive":
                    // Verificar se o persona tem passagem na polícia
                    // Se tiver passagem não deixar fazer testdrive

                    API.triggerClientEvent(player, "cancelarconce");
                    API.sendChatMessageToPlayer(player, "Você tem 3 minutos para sentir o prazer de dirigí-lo e então volte à concessionária!");
                    EnviarMensagemSubtitulo(player, string.Format("Você inicou o test-drive com o veículo ~b~{0}.", veiculo.Nome), 5);

                    var vehcr = API.createVehicle(vehc, new Vector3(-27.936, -1082.56, 26.3151), new Vector3(0.835335, -1.9803, 72), 0, 0, 0);
                    vehcr.engineStatus = true;
                    //; API.setVehicleModColor1(vehcr, cor1.red, cor1.green, cor1.blue);
                    //; API.setVehicleModColor2(vehcr, cor2.red, cor2.green, cor2.blue);
                    API.setVehicleLocked(vehcr, true);
                    API.setEntitySyncedData(player, "vehconcetest", vehcr);

                    if (player != null) API.setPlayerIntoVehicle(player, vehcr, -1);

                    var ped = API.createPed(PedHash.Business03AMY, new Vector3(), 0, 0);
                    API.sendNativeToPlayer(player, Hash.SET_PED_INTO_VEHICLE, ped, vehcr, 0);
                    API.setEntitySyncedData(player, "pedconce", ped);

                    var v = new Veiculo();
                    v.Modelo = veiculo.ToString();
                    //; v.Cor1 = cor1;
                    //; v.Cor2 = cor2;
                    v.Posicao = API.getEntityPosition(vehcr);
                    v.Rotacao = API.getEntityRotation(vehcr);
                    v.Veh = vehcr;
                    v.Placa = "TESTDRIVE";
                    API.setVehicleNumberPlate(vehcr, v.Placa);

                    v.ID = -1;
                    API.setEntitySyncedData(vehcr, "id", v.ID);

                    API.delay(180000, true, () =>
                    {
                        if (API.hasEntitySyncedData(player, "vehconcetest"))
                        {
                            // Se passar os três minutos e não tiver devolvido tem que botar um APB na PD e deletar o PED
                            EnviarMensagemSubtitulo(player, "O seu tempo de test-drive acabou.");

                            var pedcc = (NetHandle)API.getEntitySyncedData(player, "pedconce");
                            API.deleteEntity(pedcc);

                            API.deleteEntity(vehcr);

                            API.resetEntitySyncedData(player, "pedconce");
                            API.resetEntitySyncedData(player, "vehconcetest");
                        }
                    });

                    veiculos.Add(v);
                    break;
                /*case "cor1":
                    API.setVehicleCustomPrimaryColor(veh, r, g, b);
                    break;
                case "cor2":
                    API.setVehicleCustomSecondaryColor(veh, r, g, b);
                    break;*/
                case "comprar":
                    if (veiculo.Preco > API.getEntitySyncedData(player, "dinheiro"))
                    {
                        EnviarMensagemErro(player, string.Format("Você não possui dinheiro suficiente. (${0})", veiculo.Preco));
                        return;
                    }

                    foreach (var conce in concessionarias)
                    {
                        if (conce.ID == API.getEntitySyncedData(player, "ConceID"))
                        {
                            API.setEntitySyncedData(player, "ConceID", 0);
                            API.triggerClientEvent(player, "cancelarconce");
                            API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - veiculo.Preco);
                            EnviarMensagemSubtitulo(player, string.Format("Você comprou o veículo ~b~{0}~w~ por ~g~${1}.", veiculo.Nome, veiculo.Preco), 5);
                            CriarVeiculo(null, vehc, 0, 0, conce.Posicao_vspawn, new Vector3(-0.0165237, 0.0608381, 3), 0, API.getEntitySyncedData(player, "id"));
                            break;
                        }

                    }
                    break;
            }
        }
    }
    #endregion

    #region ItensDropados
    private void CarregarItensDropados()
    {
        var cmd = new MySqlCommand("SELECT * FROM itens_dropados", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var p = new ItensDropados();
            p.ID = isInt(dr["ID"].ToString());
            p.Modelo = dr["Modelo"].ToString();
            p.Quantidade = isInt(dr["Quantidade"].ToString());
            p.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            p.Rotacao = new Vector3(isFloat(dr["PosRX"].ToString()), isFloat(dr["PosRY"].ToString()), isFloat(dr["PosRZ"].ToString()));
            p.TipoItem = (ItensDropados.Tipo)isInt(dr["Tipo"].ToString());

            if (p.TipoItem == ItensDropados.Tipo.Arma)
            {
                int objHash = GetWeaponObjByName(p.Modelo);
                if (objHash != 99)
                {
                    p.Objeto = API.createObject(objHash, p.Posicao, p.Rotacao, 0);
                }
                else
                {
                    API.consoleOutput("[ITEM DROPADO] - ARMA SEM MODELO.");
                }
            }
            else
            {
                p.Objeto = API.createObject(1246356548, p.Posicao, p.Rotacao, 0);
            }
            API.setEntityPositionFrozen(p.Objeto, false);

            itensDropados.Add(p);
        }
        dr.Close();

        API.consoleOutput("Itens Dropados: {0}", itensDropados.Count);
    }
    private void DroparItem(Client player, int tipo, string modelo, int quantidade)
    {
        int objHash = GetWeaponObjByName(modelo);
        if (tipo == 2)
        {
            if (objHash == 99)
            {
                EnviarMensagemErro(player, "Erro #40209 - Contate um developer.");
                return;
            }
        }

        float z_rot = 0;
        if (tipo == 2) z_rot = 90;

        var str = string.Format(@"INSERT INTO itens_dropados (PosX, PosZ, PosY, PosRX, PosRZ, PosRY, Tipo, Modelo, Quantidade) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}', {8})",
            aFlt(player.position.X),
            aFlt(player.position.Z - 1),
            aFlt(player.position.Y),
            z_rot,
            aFlt(player.rotation.Z),
            aFlt(player.rotation.Y),
            tipo,
            modelo,
            quantidade
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var p = new ItensDropados();
        p.ID = id;
        p.Posicao = new Vector3(player.position.X, player.position.Y, player.position.Z - 1);
        p.Rotacao = new Vector3(z_rot, player.rotation.Y, player.rotation.Z);
        p.Modelo = modelo;
        p.TipoItem = (ItensDropados.Tipo)tipo;

        //API.sendChatMessageToPlayer(player, p.TipoItem.ToString());

        if (tipo == 1)
        {
            p.Quantidade = quantidade;
            API.sendNotificationToPlayer(player, "Você dropou $" + quantidade + ".");
            p.Objeto = API.createObject(1246356548, p.Posicao, p.Rotacao, 0);
        }
        else if (tipo == 2)
        {
            API.sendNotificationToPlayer(player, "Você dropou um(a) " + modelo + " com " + quantidade + " de munição.");
            p.Quantidade = quantidade;
            p.Objeto = API.createObject(objHash, p.Posicao, p.Rotacao, 0);
            API.removePlayerWeapon(player, API.weaponNameToModel(modelo));
            API.removeAllPlayerWeapons(player);
        }
        API.setEntityPositionFrozen(p.Objeto, false);
        itensDropados.Add(p);
    }

    public int GetWeaponObjByName(string WepName)
    {
        int return_w = 0;

        switch (WepName)
        {
            //Armas Brancas
            case "Knife": return_w = -518344816; break;
            case "Nightstick": return_w = -1634978236; break;
            case "Hammer": return_w = 64104227; break;
            case "Bat": return_w = 32653987; break;
            case "Crowbar": return_w = 1862268168; break;
            case "Golfclub": return_w = -580196246; break;
            case "Bottle": return_w = 1150762982; break;
            case "Dagger": return_w = 601713565; break;
            case "Hatchet": return_w = 1653948529; break;
            case "KnuckleDuster": return_w = 99; break;
            case "Machete": return_w = -2055486531; break;
            case "Flashlight": return_w = 99; break;
            case "SwitchBlade": return_w = 99; break;
            case "Poolcue": return_w = 99; break;
            case "Wrench": return_w = 99; break;
            case "Battleaxe": return_w = 99; break;
            //Pistolas
            case "Pistol": return_w = 1467525553; break;
            case "CombatPistol": return_w = 905830540; break;
            case "Pistol50": return_w = -178484015; break;
            case "SNSPistol": return_w = 339962010; break;
            case "HeavyPistol": return_w = 1927398017; break;
            case "VintagePistol": return_w = -1124046276; break;
            case "MarksmanPistol": return_w = 99; break;
            case "Revolver": return_w = 99; break;
            case "APPistol": return_w = 905830540; break;
            case "StunGun": return_w = 1609356763; break;
            case "FlareGun": return_w = 1349014803; break;
            //Machine Guns
            case "MicroSMG": return_w = -1056713654; break;
            case "MachinePistol": return_w = 99; break;
            case "SMG": return_w = -500057996; break;
            case "AssaultSMG": return_w = -473574177; break;
            case "CombatPDW": return_w = 99; break;
            case "MG": return_w = -2056364402; break;
            case "CombatMG": return_w = -739394447; break;
            case "Gusenberg": return_w = 99; break;
            case "MiniSMG": return_w = 99; break;
            //Assault Rifles
            case "AssaultRifle": return_w = 273925117; break;
            case "CarbineRifle": return_w = 1026431720; break;
            case "AdvancedRifle": return_w = -17075849; break;
            case "SpecialCarbine": return_w = 99; break;
            case "BullpupRifle": return_w = 99; break;
            case "CompactRifle": return_w = 99; break;
            //Sniper Rifles
            case "SniperRifle": return_w = 346403307; break;
            case "HeavySniper": return_w = -746966080; break;
            case "MarksmanRifle": return_w = -1711248638; break;
            //Shotguns
            case "PumpShotgun": return_w = 689760839; break;
            case "SawnoffShotgun": return_w = -675841386; break;
            case "BullpupShotgun": return_w = -1598212834; break;
            case "AssaultShotgun": return_w = 1255410010; break;
            case "Musket": return_w = 99; break;
            case "HeavyShotgun": return_w = -1209868881; break;
            case "DoubleBarrelShotgun": return_w = 99; break;
            case "Autoshotgun": return_w = 99; break;
            //Heavy Weapons
            case "GrenadeLauncher": return_w = -606683246; break;
            case "RPG": return_w = 99; break;
            case "Minigun": return_w = 422658457; break;
            case "Firework": return_w = 99; break;
            case "Railgun": return_w = 99; break;
            case "HomingLauncher": return_w = 99; break;
            case "GrenadeLauncherSmoke": return_w = 99; break;
            case "CompactLauncher": return_w = 99; break;
            //Thrown Weapons
            case "Grenade": return_w = 99; break;
            case "StickyBomb": return_w = 99; break;
            case "ProximityMine": return_w = 99; break;
            case "BZGas": return_w = 99; break;
            case "Molotov": return_w = 99; break;
            case "FireExtinguisher": return_w = 99; break;
            case "PetrolCan": return_w = 99; break;
            case "Flare": return_w = 99; break;
            case "Ball": return_w = 99; break;
            case "Snowball": return_w = 99; break;
            case "SmokeGrenade": return_w = 99; break;
            case "Pipebomb": return_w = 99; break;
            //========================================
            default: return_w = 99; break;
        }

        return return_w;
    }
    #endregion

    #region Pontos
    private void CarregarPontos()
    {
        var cmd = new MySqlCommand("SELECT * FROM pontos", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var p = new Ponto();
            p.ID = isInt(dr["ID"].ToString());
            p.Descricao = dr["Descricao"].ToString();
            p.TipoPonto = (Ponto.Tipo)isInt(dr["TipoPonto"].ToString());
            p.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            p.Text = API.createTextLabel(p.Descricao, p.Posicao, 30, (float)0.4);
            pontos.Add(p);
        }
        dr.Close();

        API.consoleOutput("Pontos: {0}", pontos.Count);
    }

    private void CriarPonto(Client player, int tipo)
    {
        var str = string.Format(@"INSERT INTO pontos (PosX, PosZ, PosY, TipoPonto, Descricao) VALUES ({0}, {1}, {2}, {3}, {4})",
            aFlt(player.position.X),
            aFlt(player.position.Z),
            aFlt(player.position.Y),
            tipo,
            at(recuperarDescricaoPorTipo(tipo))
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var p = new Ponto();
        p.ID = id;
        p.Posicao = player.position;
        p.Descricao = recuperarDescricaoPorTipo(tipo);
        p.TipoPonto = (Ponto.Tipo)tipo;
        p.Text = API.createTextLabel(p.Descricao, p.Posicao, 30, (float)0.4);

        pontos.Add(p);
    }

    private string recuperarDescricaoPorTipo(int tipo)
    {
        var descricao = string.Empty;
        switch (tipo)
        {
            case 1: descricao = "[/comprarskin | /comprarvariacao]"; break;
            case 2: descricao = "[/comprarveiculo]"; break;
            case 3: descricao = "[/uniforme]"; break;
        }
        return descricao;
    }
    #endregion

    #region Facções
    private void CarregarFaccoes()
    {
        var cmd = new MySqlCommand("SELECT * FROM faccoes ORDER BY id", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var faccao = new Faccao();
            faccao.ID = isInt(dr["ID"].ToString());
            faccao.NomeFaccao = dr["NomeFaccao"].ToString();
            faccao.AbreviaturaFaccao = dr["AbreviaturaFaccao"].ToString();
            faccao.TipoFaccao = (Faccao.Tipo)isInt(dr["TipoFaccao"].ToString());
            faccao.CorFaccao = dr["CorFaccao"].ToString();
            faccao.IDRankGestor = isInt(dr["IDRankGestor"].ToString());
            faccao.IDRankLider = isInt(dr["IDRankLider"].ToString());
            faccoes.Add(faccao);
        }
        dr.Close();

        // preciso percorrer dnv pois não aceita dois dr abertos simultaneos
        foreach (var fac in faccoes)
        {
            var cmd2 = new MySqlCommand(string.Format("SELECT * FROM ranks WHERE IDFaccao = {0} ORDER BY ID", fac.ID), bancodados);
            var dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                var rank = new Faccao.Rank();
                rank.ID = isInt(dr2["ID"].ToString());
                rank.NomeRank = dr2["NomeRank"].ToString();
                rank.Salario = isInt(dr2["Salario"].ToString());
                fac.Ranks.Add(rank);
            }
            dr2.Close();
        }

        API.consoleOutput("Facções: {0}", faccoes.Count);
    }

    private void CriarFaccao(string nomeFaccao)
    {
        var str = string.Format(@"INSERT INTO faccoes (NomeFaccao, TipoFaccao, IDRankGestor, IDRankLider) VALUES ({0}, 1, {1}, {1})", at(nomeFaccao), MAX_FACCOES_RANKS);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var fac = new Faccao();
        fac.ID = id;
        fac.NomeFaccao = nomeFaccao;
        fac.TipoFaccao = Faccao.Tipo.Civil;

        for (var i = 1; i <= MAX_FACCOES_RANKS; i++)
        {
            str = string.Format(@"INSERT INTO ranks (IDFaccao, ID, NomeRank) VALUES ({0}, {1}, {2})", id, i, at("Rank " + i));
            var cmd2 = new MySqlCommand(str, bancodados);
            cmd2.ExecuteNonQuery();
            fac.Ranks.Add(new Faccao.Rank { ID = i, NomeRank = "Rank " + i });
        }

        faccoes.Add(fac);
    }
    #endregion

    #region Pets
    private void CarregarPets()
    {
        var cmd = new MySqlCommand("SELECT * FROM pets", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var pet = new Pet();
            pet.ID = isInt(dr["id"].ToString());
            pet.IDPersonagemProprietario = isInt(dr["IDPersonagemProprietario"].ToString());
            pet.Nome = dr["Nome"].ToString();
            pet.NomePersonagemProprietario = dr["NomeOwner"].ToString();
            pet.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            pet.Sentado = 1;
            pet.Seguindo = 0;
            pet.Skin = (PedHash)isLong(dr["Skin"].ToString());
            pet.Dimensao = isInt(dr["Dimensao"].ToString());

            pet.Ped = API.createPed(pet.Skin, pet.Posicao, pet.Dimensao);
            API.setEntityInvincible(pet.Ped, false);
            API.setEntityPositionFrozen(pet.Ped, true);
            API.playPedScenario(pet.Ped, "WORLD_DOG_SITTING_ROTTWEILER");

            API.setEntityPosition(pet.Ped, pet.Posicao);

            pets.Add(pet);
        }
        dr.Close();

        API.consoleOutput("Pets: {0}", pets.Count);
    }

    private void CriarPet(Client player, int tipo, string Nome)
    {
        PedHash skin = new PedHash();
        switch (tipo)
        {
            case 1: skin = PedHash.Husky; break;
            case 2: skin = PedHash.Rottweiler; break;
            case 3: skin = PedHash.Pug; break;
            case 4: skin = PedHash.Retriever; break;
            case 5: skin = PedHash.Shepherd; break;
            case 6: skin = PedHash.Westy; break;
        }
        var pet = new Pet();

        pet.IDPersonagemProprietario = API.getEntitySyncedData(player, "id");
        pet.Dimensao = 0;
        pet.Nome = Nome;
        pet.NomePersonagemProprietario = player.name;
        pet.Posicao = player.position;
        pet.Sentado = 0;
        pet.Seguindo = API.getEntitySyncedData(player, "id");
        pet.Skin = skin;
        pet.Ped = API.createPed(pet.Skin, pet.Posicao, pet.Dimensao);

        var str = string.Format(@"INSERT INTO pets (IDPersonagemProprietario, Nome, NomeOwner, Skin) VALUES ({0}, {1}, {2}, {3})",
            API.getEntitySyncedData(player, "id"),
            at(Nome),
            at(player.name),
            pet.Ped.model
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;
        pet.ID = id;

        API.setEntityInvincible(pet.Ped, false);
        API.setEntityPositionFrozen(pet.Ped, false);

        API.setEntitySyncedData(player, "DogentrandoVeh", 0);
        API.setEntitySyncedData(player, "TemDog", 1);
        API.setEntitySyncedData(player, "mydog", pet.Ped);
        API.setEntitySyncedData(player, "NomeDoDog", pet.Nome);
        API.setEntitySyncedData(player, "Dog_ID", pet.ID);

        EnviarMensagemSucesso(player, "Agora o " + pet.Nome + " está te seguindo.");

        pets.Add(pet);
    }

    [Command("irpet")]
    public void CMD_IrPet(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 3)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var prop = recuperarPetPorID(id);
        if (prop == null)
        {
            EnviarMensagemErro(player, "Pet inválido.");
            return;
        }


        API.setEntityPosition(player, prop.Posicao);
        EnviarMensagemSucesso(player, string.Format("Você foi até o Pet {0}.", prop.ID));
    }
    [Command("trazerpet")]
    public void CMD_TrazerPet(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 3)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pet = recuperarPetPorID(id);
        if (pet == null)
        {
            EnviarMensagemErro(player, "Pet inválido.");
            return;
        }

        pet.Posicao = player.position;
        pet.Dimensao = player.dimension;
        API.setEntityPosition(pet.Ped, player.position);
        EnviarMensagemSucesso(player, string.Format("Você trouxe o Pet {0} até você.", pet.ID));
    }

    [Command("seguir")]
    public void cachorro_seguir(Client player)
    {
        foreach (var pet in pets)
        {
            if (pet.Seguindo == 0 && pet.IDPersonagemProprietario == API.getEntitySyncedData(player, "id"))
            {
                var play = API.getPlayersInRadiusOfPosition(5, pet.Posicao).Where(p => p.name.Equals(player.name)).ToList();
                if (play.Count > 0)
                {
                    API.setEntitySyncedData(player, "DogIndoAtacar", "nc");
                    API.setEntitySyncedData(player, "DogentrandoVeh", 0);
                    API.setEntitySyncedData(player, "TemDog", 1);
                    API.setEntitySyncedData(player, "mydog", pet.Ped);
                    API.setEntitySyncedData(player, "NomeDoDog", pet.Nome);
                    API.setEntitySyncedData(player, "Dog_ID", pet.ID);

                    API.stopPedAnimation(pet.Ped);

                    pet.Seguindo = 0;
                    pet.Sentado = 0;

                    API.setEntityPositionFrozen(pet.Ped, false);
                    API.setEntityInvincible(pet.Ped, false);

                    EnviarMensagemSucesso(player, "Agora o " + pet.Nome + " está te seguindo.");
                    return;
                }
            }
        }
    }
    private void timerSegundos_Elapsed(object source, ElapsedEventArgs e)
    {
        API.setTime(DateTime.Now.Hour, DateTime.Now.Minute);
        API.setWorldSyncedData("horario", string.Format("{0}:{1}", DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0')));

        VerificarGasolinaVeiculos();//Gasolina System

        var ps = API.getAllPlayers();
        foreach (var player in ps)
        {
            if (!API.hasEntitySyncedData(player, "logado"))
                continue;



            //Cachorro seguindo para matar
            if (API.hasEntitySyncedData(player, "DogVindoAtacar"))
            {
                if (API.getEntitySyncedData(player, "DogVindoAtacar") != 0)
                {
                    API.sendNotificationToPlayer(player, "Tem cachorro seguindo : MATAR");

                    Vector3 pos_cao = API.getEntitySyncedData(player, "DogMeAtacandoPos");

                    var posx = player.position.X - pos_cao.X;
                    var posy = player.position.Y - pos_cao.Y;
                    var posz = player.position.Z - pos_cao.Z;
                    var pos_f = (posx * posx) + (posy * posy) + (posz * posz);
                    var pos_result = Math.Sqrt(pos_f);

                    Pet pet = recuperarPetPorID(API.getEntitySyncedData(player, "DogVindoAtacar"));
                    pet.Posicao = pos_cao;

                    if (pos_result > 35)
                    {
                        string idOrName = API.getEntitySyncedData(player, "Dono_DogMeAtacando");

                        var target = findPlayer(player, idOrName);
                        if (target == null)
                        {
                            API.sendNativeToAllPlayers(Hash.CLEAR_PED_TASKS, pet.Ped);
                        }
                        else
                        {
                            EnviarMensagemSucesso(target, "Fugiu do seu cachorro.");
                            API.sendNativeToAllPlayers(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY, pet.Ped, target, 0, 0, 0, 1, -1, 1.0, true);
                            API.setEntitySyncedData(target, "DogIndoAtacar", "nc");
                        }
                        pet.Seguindo = 0;
                        EnviarMensagemSucesso(player, "Você fugiu do cachorro.");
                        API.setEntitySyncedData(player, "Dono_DogMeAtacando", null);
                        API.setEntitySyncedData(player, "DogVindoAtacar", 0);
                        API.setEntitySyncedData(player, "DogVindoMeAtacar", null);
                    }
                    else
                        EnviarMensagemSucesso(player, "Cachorro te seguindo.");
                }
            }
            if (!API.isPlayerInAnyVehicle(player))
            {
                //=================

                //=================
                if (API.getEntitySyncedData(player, "TemDog") == 1 && API.getEntitySyncedData(player, "DogentrandoVeh") == 0 && API.getEntitySyncedData(player, "DogIndoAtacar") == "nc")
                {
                    API.setEntityInvincible(API.getEntitySyncedData(player, "mydog"), false);
                    Vector3 pos_cao = API.getEntitySyncedData(player, "DogPos");

                    if (!API.hasEntitySyncedData(player, "DogIndoPos"))
                        API.setEntitySyncedData(player, "DogIndoPos", 0);

                    Pet pet = recuperarPetPorID(API.getEntitySyncedData(player, "Dog_ID"));
                    pet.Posicao = pos_cao;

                    API.sendNotificationToPlayer(player, "Tem cachorro seguindo : SEGUIR : " + pet.ID + " IndoAtacar: " + API.getEntitySyncedData(player, "DogIndoAtacar"));

                    var posx = player.position.X - pos_cao.X;
                    var posy = player.position.Y - pos_cao.Y;
                    var posz = player.position.Z - pos_cao.Z;
                    var pos_f = (posx * posx) + (posy * posy) + (posz * posz);
                    var pos_result = Math.Sqrt(pos_f);

                    if (pos_result < 7)
                    {
                        API.sendNativeToAllPlayers(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY, pet.Ped, player, 0, 0, 0, 1, -1, 1.0, true);
                        API.setEntitySyncedData(player, "DogIndoPos", 0);
                    }
                    else if (pos_result >= 7 && pos_result < 20)
                    {
                        if (API.getEntitySyncedData(player, "DogIndoPos") == 0)
                        {
                            API.setEntitySyncedData(player, "DogIndoPos", 1);
                            API.sendNativeToAllPlayers(Hash.TASK_GO_TO_ENTITY, pet.Ped, player, -1, 0, 5.0, 0, 0); // 6A071245EB0D1882 374827C2
                        }
                    }
                    else
                    {
                        API.setEntitySyncedData(player, "DogIndoPos", 0);
                        API.sendNativeToAllPlayers(Hash.TASK_FOLLOW_TO_OFFSET_OF_ENTITY, pet.Ped, player, 0, 0, 0, 1, -1, 1.0, true);
                        API.sendNotificationToPlayer(player, "Você foi para muito longe do seu cachorro " + API.getEntitySyncedData(player, "NomeDoDog") + "...\nVolte para pega-lo.");
                    }
                }

                //========================================

            }
            else
            {
                var veh = (Veiculo)recuperarVeiculoPorID(API.getEntitySyncedData(player.vehicle, "id"));
                PlayerAbastecendo(player);
                API.triggerClientEvent(player, "gasolinaVeh", veh.gasolina);
            }

        }
    }
    private Pet recuperarPetPorID(int id)
    {
        var prop = pets.Where(p => p.ID.Equals(id)).ToList();
        if (prop.Count > 0)
            return prop[0];

        return null;
    }
    private void SalvarPet(Pet pet, float px, float py, float pz)
    {
        var petpos = pet.Posicao;
        var dimensao = pet.Dimensao;

        API.consoleOutput("PET: " + pet.ID + " | POS: " + pet.Posicao);

        var str = string.Format(@"UPDATE pets SET PosX={1}, PosY={2}, PosZ={3}, dimensao={4} WHERE id = {0}",
            pet.ID,
            px.ToString().Replace(",", "."),
            py.ToString().Replace(",", "."),
            pz.ToString().Replace(",", "."),
            dimensao
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

    }

    public void DogSystem_Inicializar(Client player)
    {
        API.setEntitySyncedData(player, "TemDog", 0);
        API.setEntitySyncedData(player, "mydog", null);
        API.setEntitySyncedData(player, "NomeDoDog", string.Empty);
        API.setEntitySyncedData(player, "Dog_ID", 0);
        API.setEntitySyncedData(player, "DogIndoAtacar", "nc");
        API.setEntitySyncedData(player, "Dono_DogMeAtacando", string.Empty);
        API.setEntitySyncedData(player, "DogVindoAtacar", 0);
        API.setEntitySyncedData(player, "DogVindoMeAtacar", null);
    }
    [Command("atacar")]
    public void CMD_CachorroAtacar(Client player, string idOrName = "")
    {
        if (API.hasEntitySyncedData(player, "TemDog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") == 1)
            {
                if (!API.hasEntitySyncedData(player, "DogIndoAtacar"))
                    API.setEntitySyncedData(player, "DogIndoAtacar", "nc");

                if (API.getEntitySyncedData(player, "DogentrandoVeh") != 0)
                {
                    EnviarMensagemErro(player, "Seu cachorro está em um veículo, tire-o antes.");
                    return;
                }

                if (idOrName == "")
                {
                    Pet pet = recuperarPetPorID(API.getEntitySyncedData(player, "Dog_ID"));
                    pet.Seguindo = 0;

                    string pet_atacando = API.getEntitySyncedData(player, "DogIndoAtacar");

                    API.setEntitySyncedData(player, "DogIndoAtacar", "nc");
                    //=========================================================================================

                    API.sendNativeToAllPlayers(Hash.CLEAR_PED_TASKS, pet.Ped);

                    var targett = findPlayer(player, pet_atacando);
                    if (targett == null)
                    {
                        EnviarMensagemErro(player, "Jogador não encontrado.");
                        return;
                    }

                    API.setEntitySyncedData(targett, "Dono_DogMeAtacando", null);
                    API.setEntitySyncedData(targett, "DogVindoAtacar", 0);
                    API.setEntitySyncedData(targett, "DogVindoMeAtacar", pet.Ped);
                    //=========================================================================================

                    EnviarMensagemSucesso(player, "Você ordenou que " + pet.Nome + " pare a perseguição.");
                    return;
                }
                var target = findPlayer(player, idOrName);
                if (target == null)
                {
                    EnviarMensagemErro(player, "Jogador não encontrado.");
                    return;
                }
                /*else if (player == target)
                {
                    EnviarMensagemErro(player, "Você não pode pagar para si mesmo.");
                    return;
                }*/

                var players = API.getPlayersInRadiusOfPlayer(20, player).Where(p => p.name.Equals(target.name)).ToList();
                if (players.Count == 1)
                {
                    int dog_id = API.getEntitySyncedData(player, "Dog_ID");
                    Pet pet = recuperarPetPorID(dog_id);
                    pet.Seguindo = 1;

                    API.setEntitySyncedData(player, "DogIndoAtacar", idOrName);
                    //=========================================================================================
                    API.sendNotificationToPlayer(player, "Debug 2");

                    API.setEntitySyncedData(target, "Dono_DogMeAtacando", player.name);
                    API.setEntitySyncedData(target, "DogVindoAtacar", dog_id);
                    API.setEntitySyncedData(target, "DogVindoMeAtacar", pet.Ped);

                    API.sendNotificationToPlayer(target, "[CÃO] DOG ID: " + dog_id + " | ID SALVO: " + API.getEntitySyncedData(target, "DogVindoAtacar") + " | ID SALVO PLAYER: " + API.getEntitySyncedData(player, "DogVindoAtacar"));
                    //=========================================================================================
                    API.sendNativeToAllPlayers(Hash.TASK_COMBAT_PED, pet.Ped, target, 0, 1); // 0E46A3FCBDE2A1B1 46AFFED3
                    return;
                }
                else
                {
                    EnviarMensagemErro(player, string.Format("Você não está perto de {0}.", target.name.Replace("_", " ")));
                    return;
                }
            }
        }
        EnviarMensagemErro(player, "Você não está com um cachorro.");
    }
    [Command("entrarv")]
    public void CMD_dogentrarcarro(Client player, int porta)
    {
        if (API.hasEntitySyncedData(player, "TemDog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") == 1)
            {
                if (API.getEntitySyncedData(player, "DogentrandoVeh") != 0)
                {
                    EnviarMensagemErro(player, "Seu cachorro já está em um veículo.");
                    return;
                }
                if (API.getEntitySyncedData(player, "DogIndoAtacar") != "nc")
                {
                    EnviarMensagemErro(player, "Seu cachorro está perseguindo alguém.");
                    return;
                }

                foreach (var veh in API.getAllVehicles())
                {
                    var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
                    if (play.Count > 0)
                    {
                        if (API.getEntitySyncedData(player, "id") != recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id")).IDPersonagemProprietario)
                        {
                            EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                            return;
                        }
                        if (API.getVehicleLocked(veh))
                        {
                            EnviarMensagemErro(player, "O veículo está trancado.");
                            return;
                        }

                        API.setEntitySyncedData(player, "DogentrandoVeh", recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id")).ID);
                        API.setEntitySyncedData(player, "DogNaPorta", porta);

                        API.sendNativeToPlayersInRange(player.position, 50, Hash.TASK_OPEN_VEHICLE_DOOR, player, veh, 5000, porta, 2);

                        var pedcc = (NetHandle)API.getEntitySyncedData(player, "mydog");
                        API.sendNativeToPlayersInRange(player.position, 50, Hash.TASK_ENTER_VEHICLE, pedcc, veh, -1, porta, 1.0, 1, 0);

                        return;
                    }
                }
            }
        }
        EnviarMensagemErro(player, "Você não está com nenhum cachorro.");
    }
    [Command("sairv")]
    public void CMD_dogsaircarro(Client player)
    {
        if (API.hasEntitySyncedData(player, "TemDog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") == 1)
            {
                if (API.getEntitySyncedData(player, "DogentrandoVeh") == 0)
                {
                    EnviarMensagemErro(player, "Seu cachorro não está em um veículo.");
                    return;
                }
                foreach (var veh in API.getAllVehicles())
                {
                    var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
                    if (play.Count > 0)
                    {
                        if (API.getEntitySyncedData(player, "id") != recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id")).IDPersonagemProprietario)
                        {
                            EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                            return;
                        }
                        if (API.getEntitySyncedData(player, "DogentrandoVeh") != recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id")).ID)
                        {
                            EnviarMensagemErro(player, "Seu cachorro não está em neste veículo.");
                            return;
                        }
                        if (API.getVehicleLocked(veh))
                        {
                            EnviarMensagemErro(player, "O veículo está trancado.");
                            return;
                        }

                        API.sendNativeToPlayersInRange(player.position, 50, Hash.TASK_OPEN_VEHICLE_DOOR, player, veh, 5000, API.getEntitySyncedData(player, "DogNaPorta"), 2);

                        API.delay(5000, true, () =>
                        {
                            var pedcc = (NetHandle)API.getEntitySyncedData(player, "mydog");
                            API.sendNativeToPlayer(player, Hash.TASK_LEAVE_VEHICLE, pedcc, veh, 0);

                            API.setEntitySyncedData(player, "DogentrandoVeh", 0);
                            API.setEntitySyncedData(player, "DogNaPorta", -1);
                        });
                        return;
                    }
                }
            }
        }
        EnviarMensagemErro(player, "Você não está com nenhum cachorro.");
    }

    [Command("ficar")]
    public void cachorro_ficar(Client player)
    {
        if (API.hasEntitySyncedData(player, "TemDog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") == 1)
            {
                Pet pet = recuperarPetPorID(API.getEntitySyncedData(player, "Dog_ID"));

                if (pet.Seguindo == 0)
                {
                    var play = API.getPlayersInRadiusOfPosition(2, pet.Posicao).Where(p => p.name.Equals(player.name)).ToList();
                    if (play.Count > 0)
                    {
                        API.setEntityPositionFrozen(pet.Ped, true);
                        API.setEntityInvincible(pet.Ped, false);

                        pet.Seguindo = 0;
                        pet.Sentado = 1;

                        Vector3 pos_cao = API.getEntitySyncedData(player, "DogPos");

                        if (!API.hasEntitySyncedData(player, "DogPos"))
                            EnviarMensagemErro(player, "Erro #4995 - Contate o Freeze");
                        else
                        {
                            pet.Posicao = pos_cao;

                            API.consoleOutput("Posicao: " + pet.Posicao);
                        }
                        SalvarPet(pet, pos_cao.X, pos_cao.Y, pos_cao.Z);

                        API.playPedScenario(pet.Ped, "WORLD_DOG_SITTING_ROTTWEILER");

                        API.setEntitySyncedData(player, "DogIndoAtacar", "nc");
                        API.setEntitySyncedData(player, "TemDog", 0);
                        API.setEntitySyncedData(player, "mydog", null);
                        API.setEntitySyncedData(player, "NomeDoDog", string.Empty);
                        API.setEntitySyncedData(player, "Dog_ID", 0);

                        EnviarMensagemSucesso(player, "Você mandou o " + pet.Nome + " ficar no lugar. (ID: " + pet.ID + ")");
                        return;
                    }
                    else
                    {
                        EnviarMensagemErro(player, "Você não está próximo do seu cachorro."); return;
                    }
                }
                else
                {
                    EnviarMensagemErro(player, "Seu cachorro está perseguindo alguém agora.."); return;
                }
            }
        }
        EnviarMensagemErro(player, "Você não está com nenhum cachorro.");
    }

    [Command("ccao")]
    public void ccao(Client player, int cachorroid, string nome)
    {
        CriarPet(player, cachorroid, nome);
    }
    #endregion

    #region Propriedades
    private void CarregarPropriedades()
    {
        var cmd = new MySqlCommand("SELECT propriedades.*, personagens.NomePersonagem FROM propriedades LEFT JOIN personagens ON personagens.ID = propriedades.IDPersonagemProprietario", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var prop = new Propriedade();
            prop.ID = isInt(dr["ID"].ToString());
            prop.Endereco = dr["Endereco"].ToString();
            prop.Level = isInt(dr["Level"].ToString());
            prop.TipoPropriedade = (Propriedade.Tipo)isInt(dr["TipoPropriedade"].ToString());
            prop.Interior = isInt(dr["Interior"].ToString());
            prop.EntradaFrente = new Vector3(isFloat(dr["EntradaFrentePosX"].ToString()), isFloat(dr["EntradaFrentePosY"].ToString()), isFloat(dr["EntradaFrentePosZ"].ToString()));
            prop.SaidaFrente = new Vector3(isFloat(dr["SaidaFrentePosX"].ToString()), isFloat(dr["SaidaFrentePosY"].ToString()), isFloat(dr["SaidaFrentePosZ"].ToString()));
            prop.IDPersonagemProprietario = isInt(dr["IDPersonagemProprietario"].ToString());
            prop.ValorPropriedade = isInt(dr["ValorPropriedade"].ToString());
            prop.IDPropriedade = isInt(dr["IDPropriedade"].ToString());
            prop.NomePersonagemProprietario = dr["NomePersonagem"].ToString().Replace("_", " ");
            prop.IDFaccao = isInt(dr["IDFaccao"].ToString());

            if (prop.IDPropriedade == 0)
            {
                var r = 29;
                var g = 153;
                var b = 194;

                if (prop.IDPersonagemProprietario == 0)
                {
                    r = 109;
                    g = 242;
                    b = 120;
                }

                prop.TextFrente = API.createTextLabel(string.Format("nº {0}, {1}", prop.ID, prop.Endereco), prop.EntradaFrente, 30, (float)0.4);
                prop.MarkerFrente = API.createMarker(20, prop.EntradaFrente, prop.EntradaFrente, new Vector3(1, 1, 1), new Vector3(0.5, 0.5, 0.5), 255, r, g, b);

                if (prop.ValorPropriedade > 0 && prop.IDPersonagemProprietario == 0)
                    prop.TextValorFrente = API.createTextLabel(string.Format("~g~${0}~w~, Level {1}", prop.ValorPropriedade, prop.Level), new Vector3(prop.EntradaFrente.X, prop.EntradaFrente.Y, prop.EntradaFrente.Z - 0.150), 30, (float)0.3);
            }

            propriedades.Add(prop);
        }
        dr.Close();

        API.consoleOutput("Propriedades: {0}", propriedades.Count);
    }

    private void CriarPropriedade(Client player)
    {
        var str = string.Format(@"INSERT INTO propriedades (EntradaFrentePosX, EntradaFrentePosY, EntradaFrentePosZ) VALUES ({0}, {1}, {2})",
            player.position.X.ToString().Replace(",", "."),
            player.position.Y.ToString().Replace(",", "."),
            player.position.Z.ToString().Replace(",", ".")
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var prop = new Propriedade();
        prop.ID = id;
        prop.EntradaFrente = player.position;
        prop.SaidaFrente = RecuperarPosicaoPorInterior(1);
        prop.TextFrente = API.createTextLabel(string.Format("nº {0}, {1}", prop.ID, prop.Endereco), prop.EntradaFrente, 30, (float)0.4);
        prop.MarkerFrente = API.createMarker(20, prop.EntradaFrente, prop.EntradaFrente, new Vector3(1, 1, 1), new Vector3(0.5, 0.5, 0.5), 255, 109, 242, 120);
        API.setTextLabelColor(prop.TextFrente, 255, 255, 255, 255);
        propriedades.Add(prop);
    }

    private Vector3 RecuperarPosicaoPorInterior(int id)
    {
        var pos = new Vector3();
        switch (id)
        {
            // Sem exterior a mostra
            case 1: pos = new Vector3(151.5138, -1007.962, -98.99999); break; // Hotel
            case 2: pos = new Vector3(1273.9, -1719.305, 54.77141); break; // Lester 
            case 3: pos = new Vector3(265.7536, -1007.297, -101.0086); break; // Baixo 1 
            case 4: pos = new Vector3(346.5572, -1013.196, -99.19623); break; // Médio 1 
            // Com exterior a mostra
            case 5: pos = new Vector3(-786.8663, 315.7642, 217.6385); break; //
            case 6: pos = new Vector3(-786.9563, 315.6229, 187.9136); break; //
            case 7: pos = new Vector3(-774.0126, 342.0428, 196.6864); break; //
            case 8: pos = new Vector3(-787.0749, 315.8198, 217.6386); break; //
            case 9: pos = new Vector3(-786.8195, 315.5634, 187.9137); break; //
            case 10: pos = new Vector3(-774.1382, 342.0316, 196.6864); break; //
            case 11: pos = new Vector3(-786.6245, 315.6175, 217.6385); break; //
            case 12: pos = new Vector3(-786.9584, 315.7974, 187.9135); break; //
            case 13: pos = new Vector3(-774.0223, 342.1718, 196.6863); break; //
            case 14: pos = new Vector3(-787.0902, 315.7039, 217.6384); break; //
            case 15: pos = new Vector3(-787.0155, 315.7071, 187.9135); break; //
            case 16: pos = new Vector3(-773.8976, 342.1525, 196.6863); break; //
            case 17: pos = new Vector3(-786.9887, 315.7393, 217.6386); break; //
            case 18: pos = new Vector3(-786.8809, 315.6634, 187.9136); break; //
            case 19: pos = new Vector3(-774.0675, 342.0773, 196.6864); break; //
            case 20: pos = new Vector3(-787.1423, 315.6943, 217.6384); break; //
            case 21: pos = new Vector3(-787.0961, 315.815, 187.9135); break; //
            case 22: pos = new Vector3(-773.9552, 341.9892, 196.6862); break; //
            case 23: pos = new Vector3(-787.029, 315.7113, 217.6385); break; //
            case 24: pos = new Vector3(-787.0574, 315.6567, 187.9135); break; //
            case 25: pos = new Vector3(-774.0109, 342.0965, 196.6863); break; //
            case 26: pos = new Vector3(-786.9469, 315.5655, 217.6383); break; //
            case 27: pos = new Vector3(-786.9756, 315.723, 187.9134); break; //
            case 28: pos = new Vector3(-774.0349, 342.0296, 196.6862); break; //
            // Escritórios
            case 29: pos = new Vector3(-141.1987, -620.913, 168.8205); break; //
            case 30: pos = new Vector3(-141.5429, -620.9524, 168.8204); break; //
            case 31: pos = new Vector3(-141.2896, -620.9618, 168.8204); break; //
            case 32: pos = new Vector3(-141.4966, -620.8292, 168.8204); break; //
            case 33: pos = new Vector3(-141.3997, -620.9006, 168.8204); break; //
            case 34: pos = new Vector3(-141.5361, -620.9186, 168.8204); break; //
            case 35: pos = new Vector3(-141.392, -621.0451, 168.8204); break; //
            case 36: pos = new Vector3(-141.1945, -620.8729, 168.8204); break; //
            case 37: pos = new Vector3(-141.4924, -621.0035, 168.8205); break; //
            // Club's and Warehouse's
            case 38: pos = new Vector3(1107.04, -3157.399, -37.51859); break; // Clubhouse 1
            case 39: pos = new Vector3(998.4809, -3164.711, -38.90733); break; // Clubhouse 1
            case 40: pos = new Vector3(1009.5, -3196.6, -38.99682); break; // Warehouse 1
            case 41: pos = new Vector3(1051.491, -3196.536, -39.14842); break; // Warehouse 2
            case 42: pos = new Vector3(1093.6, -3196.6, -38.99841); break; // Warehouse 3
            case 43: pos = new Vector3(1121.897, -3195.338, -40.4025); break; // Warehouse 4 
            case 44: pos = new Vector3(1165, -3196.6, -39.01306); break; // Warehouse 5
            case 45: pos = new Vector3(1094.988, -3101.776, -39.00363); break; // Warehouse Small
            case 46: pos = new Vector3(1056.486, -3105.724, -39.00439); break; // Warehouse Medium
            case 47: pos = new Vector3(1006.967, -3102.079, -39.0035); break; // Warehouse Large
            // Garagens
            case 48: pos = new Vector3(994.5925, -3002.594, -39.64699); break; // Cargarage
            case 49: pos = new Vector3(173.2903, -1003.6, -99.65707); break; // 2 carros
            case 50: pos = new Vector3(197.8153, -1002.293, -99.65749); break; // 6 carros
            case 51: pos = new Vector3(229.9559, -981.7928, -99.66071); break; // 10 carros
            //Maze Bank
            case 52: pos = new Vector3(-75.8466, -826.9893, 243.3859); break;
            case 53: pos = new Vector3(-75.49945, -827.05, 243.386); break;
            case 54: pos = new Vector3(-75.49827, -827.1889, 243.386); break;
            case 55: pos = new Vector3(-75.44054, -827.1487, 243.3859); break;
            case 56: pos = new Vector3(-75.63942, -827.1022, 243.3859); break;
            case 57: pos = new Vector3(-75.47446, -827.2621, 243.386); break;
            case 58: pos = new Vector3(-75.47446, -827.2621, 243.386); break;
            case 59: pos = new Vector3(-75.47446, -827.2621, 243.386); break;
            case 60: pos = new Vector3(-75.47446, -827.2621, 243.386); break;
            //Lom Bank
            case 61: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 62: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 63: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 64: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 65: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 66: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 67: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 68: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            case 69: pos = new Vector3(-1579.693, -564.8981, 108.5229); break;
            //Outros Apartamentos (SEM IPL)
            case 70: pos = new Vector3(266.2973, -1007.587, -101.0085); break;
            case 71: pos = new Vector3(-35.31277, -580.4199, 88.71221); break;
            case 72: pos = new Vector3(-1477.14, -538.7499, 55.5264); break;
            case 73: pos = new Vector3(-18.07856, -583.6725, 79.46569); break;
            case 74: pos = new Vector3(-1468.14, -541.815, 73.4442); break;
            case 75: pos = new Vector3(-915.811, -379.432, 113.6748); break;
            case 76: pos = new Vector3(-614.86, 40.6783, 97.60007); break;
            case 77: pos = new Vector3(-773.407, 341.766, 211.397); break;
            case 78: pos = new Vector3(-169.286, 486.4938, 137.4436); break;
            case 79: pos = new Vector3(340.9412, 437.1798, 149.3925); break;
            case 80: pos = new Vector3(373.023, 416.105, 145.7006); break;
            case 81: pos = new Vector3(-676.127, 588.612, 145.1698); break;
            case 82: pos = new Vector3(-763.107, 615.906, 144.1401); break;
            case 83: pos = new Vector3(-857.798, 682.563, 152.6529); break;
            case 84: pos = new Vector3(120.5, 549.952, 184.097); break;
            case 85: pos = new Vector3(-1288, 440.748, 97.69459); break;
            //Misc NO IPL
            case 86: pos = new Vector3(402.5164, -1002.847, -99.2587); break;
            case 87: pos = new Vector3(405.9228, -954.1149, -99.6627); break;
            case 88: pos = new Vector3(136.5146, -2203.149, 7.30914); break;
            case 89: pos = new Vector3(-1005.84, -478.92, 50.02733); break;
            case 90: pos = new Vector3(-1908.024, -573.4244, 19.09722); break;
            case 91: pos = new Vector3(2331.344, 2574.073, 46.68137); break;
            case 92: pos = new Vector3(-1427.299, -245.1012, 16.8039); break;
            case 93: pos = new Vector3(152.2605, -1004.471, -98.99999); break;
            case 94: pos = new Vector3(1401.21, 1146.954, 114.3337); break;
            case 95: pos = new Vector3(-1044.193, -236.9535, 37.96496); break;
            case 96: pos = new Vector3(1273.9, -1719.305, 54.77141); break;
            case 97: pos = new Vector3(134.5835, -749.339, 258.152); break;
            case 98: pos = new Vector3(134.573, -766.486, 234.152); break;
            case 99: pos = new Vector3(134.635, -765.831, 242.152); break;
            case 100: pos = new Vector3(117.22, -620.938, 206.1398); break;
            //Union Depository
            case 101: pos = new Vector3(2.6968, -667.0166, 16.13061); break;
        }
        return pos;
    }

    private string RecuperarIPLPorInterior(int id)
    {
        var ipl = string.Empty;
        switch (id)
        {
            // Com exterior a mostra
            case 5: ipl = "apa_v_mp_h_01_a"; break;
            case 6: ipl = "apa_v_mp_h_01_c"; break;
            case 7: ipl = "apa_v_mp_h_01_b"; break;
            case 8: ipl = "apa_v_mp_h_02_a"; break;
            case 9: ipl = "apa_v_mp_h_02_c"; break;
            case 10: ipl = "apa_v_mp_h_02_b"; break;
            case 11: ipl = "apa_v_mp_h_03_a"; break;
            case 12: ipl = "apa_v_mp_h_03_c"; break;
            case 13: ipl = "apa_v_mp_h_03_b"; break;
            case 14: ipl = "apa_v_mp_h_04_a"; break;
            case 15: ipl = "apa_v_mp_h_04_c"; break;
            case 16: ipl = "apa_v_mp_h_04_b"; break;
            case 17: ipl = "apa_v_mp_h_05_a"; break;
            case 18: ipl = "apa_v_mp_h_05_c"; break;
            case 19: ipl = "apa_v_mp_h_05_b"; break;
            case 20: ipl = "apa_v_mp_h_06_a"; break;
            case 21: ipl = "apa_v_mp_h_06_c"; break;
            case 22: ipl = "apa_v_mp_h_06_b"; break;
            case 23: ipl = "apa_v_mp_h_07_a"; break;
            case 24: ipl = "apa_v_mp_h_07_c"; break;
            case 25: ipl = "apa_v_mp_h_07_b"; break;
            case 26: ipl = "apa_v_mp_h_08_a"; break;
            case 27: ipl = "apa_v_mp_h_08_c"; break;
            case 28: ipl = "apa_v_mp_h_08_b"; break;
            // Escritórios
            case 29: ipl = "ex_dt1_02_office_02b"; break;
            case 30: ipl = "ex_dt1_02_office_02c"; break;
            case 31: ipl = "ex_dt1_02_office_02a"; break;
            case 32: ipl = "ex_dt1_02_office_01a"; break;
            case 33: ipl = "ex_dt1_02_office_01b"; break;
            case 34: ipl = "ex_dt1_02_office_01c"; break;
            case 35: ipl = "ex_dt1_02_office_03a"; break;
            case 36: ipl = "ex_dt1_02_office_03b"; break;
            case 37: ipl = "ex_dt1_02_office_03c"; break;
            // Club's and warehouse's
            case 38: ipl = "bkr_biker_interior_placement_interior_0_biker_dlc_int_01_milo"; break;
            case 39: ipl = "bkr_biker_interior_placement_interior_1_biker_dlc_int_02_milo"; break;
            case 40: ipl = "bkr_biker_interior_placement_interior_2_biker_dlc_int_ware01_milo"; break;
            case 41: ipl = "bkr_biker_interior_placement_interior_3_biker_dlc_int_ware02_milo"; break;
            case 42: ipl = "bkr_biker_interior_placement_interior_4_biker_dlc_int_ware03_milo"; break;
            case 43: ipl = "bkr_biker_interior_placement_interior_5_biker_dlc_int_ware04_milo"; break;
            case 44: ipl = "bkr_biker_interior_placement_interior_6_biker_dlc_int_ware05_milo"; break;
            case 45: ipl = "ex_exec_warehouse_placement_interior_1_int_warehouse_s_dlc_milo"; break;
            case 46: ipl = "ex_exec_warehouse_placement_interior_0_int_warehouse_m_dlc_milo"; break;
            case 47: ipl = "ex_exec_warehouse_placement_interior_2_int_warehouse_l_dlc_milo"; break;
            // Garagens
            case 48: ipl = "imp_impexp_interior_placement_interior_1_impexp_intwaremed_milo_"; break;
            // Maze Bank Building
            case 52: ipl = "ex_dt1_11_office_02b"; break;
            case 53: ipl = "ex_dt1_11_office_02c"; break;
            case 54: ipl = "ex_dt1_11_office_02a"; break;
            case 55: ipl = "ex_dt1_11_office_01a"; break;
            case 56: ipl = "ex_dt1_11_office_01b"; break;
            case 57: ipl = "ex_dt1_11_office_01c"; break;
            case 58: ipl = "ex_dt1_11_office_03a"; break;
            case 59: ipl = "ex_dt1_11_office_03b"; break;
            case 60: ipl = "ex_dt1_11_office_03c"; break;
            //Lom Bank
            case 61: ipl = "ex_sm_13_office_02b"; break;
            case 62: ipl = "ex_sm_13_office_02c"; break;
            case 63: ipl = "ex_sm_13_office_02a"; break;
            case 64: ipl = "ex_sm_13_office_01a"; break;
            case 65: ipl = "ex_sm_13_office_01b"; break;
            case 66: ipl = "ex_sm_13_office_01c"; break;
            case 67: ipl = "ex_sm_13_office_03a"; break;
            case 68: ipl = "ex_sm_13_office_03b"; break;
            case 69: ipl = "ex_sm_13_office_03c"; break;
            //Union Depository
            case 101: ipl = "FINBANK"; break;
        }
        return ipl;
    }

    private void RecarregarPropriedade(Client player, int id, string campo, string valor)
    {
        var prop = recuperarPropriedadePorID(id);
        var index = propriedades.IndexOf(prop);

        if (propriedades[index].ID > 0)
        {
            switch (campo)
            {
                case "endereco":
                    propriedades[index].Endereco = valor;
                    if (propriedades[index].TextFrente != null) propriedades[index].TextFrente.delete();
                    propriedades[index].TextFrente = API.createTextLabel(string.Format("nº {0}, {1}", propriedades[index].ID, propriedades[index].Endereco), propriedades[index].EntradaFrente, 30, (float)0.4);
                    break;
                case "tipo":
                    propriedades[index].TipoPropriedade = (Propriedade.Tipo)isInt(valor);
                    break;
                case "faccao":
                    propriedades[index].IDFaccao = isInt(valor);
                    break;
                case "prop":
                    propriedades[index].IDPropriedade = isInt(valor);

                    if (propriedades[index].TextValorFrente != null) propriedades[index].TextValorFrente.delete();
                    if (propriedades[index].MarkerFrente != null) propriedades[index].MarkerFrente.delete();
                    if (propriedades[index].TextFrente != null) propriedades[index].TextFrente.delete();

                    if (isInt(valor) == 0)
                    {
                        propriedades[index].TextFrente = API.createTextLabel(string.Format("nº {0}, {1}", propriedades[index].ID, propriedades[index].Endereco), propriedades[index].EntradaFrente, 30, (float)0.4);
                        if (propriedades[index].ValorPropriedade > 0 && propriedades[index].IDPersonagemProprietario == 0)
                        {
                            propriedades[index].TextValorFrente = API.createTextLabel(string.Format("~g~${0}~w~, Level {1}", propriedades[index].ValorPropriedade, propriedades[index].Level),
                                new Vector3(propriedades[index].EntradaFrente.X, propriedades[index].EntradaFrente.Y, propriedades[index].EntradaFrente.Z - 0.150), 30, (float)0.3);
                        }
                    }
                    break;
                case "interior":
                    propriedades[index].Interior = isInt(valor);
                    propriedades[index].SaidaFrente = RecuperarPosicaoPorInterior(propriedades[index].Interior);
                    break;
                case "level":
                    propriedades[index].Level = isInt(valor);
                    if (propriedades[index].TextValorFrente != null) propriedades[index].TextValorFrente.delete();
                    if (propriedades[index].ValorPropriedade > 0 && propriedades[index].IDPersonagemProprietario == 0)
                    {
                        propriedades[index].TextValorFrente = API.createTextLabel(string.Format("~g~${0}~w~, Level {1}", propriedades[index].ValorPropriedade, propriedades[index].Level),
                            new Vector3(propriedades[index].EntradaFrente.X, propriedades[index].EntradaFrente.Y, propriedades[index].EntradaFrente.Z - 0.150), 30, (float)0.3);
                    }
                    break;
                case "valor":
                    propriedades[index].ValorPropriedade = isInt(valor);
                    if (propriedades[index].TextValorFrente != null) propriedades[index].TextValorFrente.delete();
                    if (propriedades[index].ValorPropriedade > 0 && propriedades[index].IDPersonagemProprietario == 0)
                    {
                        propriedades[index].TextValorFrente = API.createTextLabel(string.Format("~g~${0}~w~, Level {1}", propriedades[index].ValorPropriedade, propriedades[index].Level),
                            new Vector3(propriedades[index].EntradaFrente.X, propriedades[index].EntradaFrente.Y, propriedades[index].EntradaFrente.Z - 0.150), 30, (float)0.3);
                    }
                    break;
                case "entradafrente":
                    propriedades[index].EntradaFrente = player.position;
                    if (propriedades[index].TextFrente != null) propriedades[index].TextFrente.delete();
                    propriedades[index].TextFrente = API.createTextLabel(string.Format("nº {0}, {1}", propriedades[index].ID, propriedades[index].Endereco), propriedades[index].EntradaFrente, 30, (float)0.4);

                    var r = 29;
                    var g = 153;
                    var b = 194;

                    if (propriedades[index].IDPersonagemProprietario == 0)
                    {
                        r = 109;
                        g = 242;
                        b = 120;
                    }
                    if (propriedades[index].MarkerFrente != null) propriedades[index].MarkerFrente.delete();
                    propriedades[index].MarkerFrente = API.createMarker(20, propriedades[index].EntradaFrente, propriedades[index].EntradaFrente, new Vector3(1, 1, 1), new Vector3(0.5, 0.5, 0.5), 255, r, g, b);
                    break;
                case "proprietario":
                    propriedades[index].IDPersonagemProprietario = isInt(valor);
                    if (propriedades[index].TextValorFrente != null) propriedades[index].TextValorFrente.delete();
                    if (propriedades[index].MarkerFrente != null) propriedades[index].MarkerFrente.delete();

                    if (propriedades[index].IDPropriedade == 0)
                    {
                        var r1 = 29;
                        var g1 = 153;
                        var b1 = 194;

                        if (propriedades[index].IDPersonagemProprietario == 0)
                        {
                            r1 = 109;
                            g1 = 242;
                            b1 = 120;
                        }

                        propriedades[index].MarkerFrente = API.createMarker(20, propriedades[index].EntradaFrente, propriedades[index].EntradaFrente, new Vector3(1, 1, 1), new Vector3(0.5, 0.5, 0.5), 255, r1, g1, b1);
                        if (propriedades[index].ValorPropriedade > 0 && propriedades[index].IDPersonagemProprietario == 0)
                        {
                            propriedades[index].TextValorFrente = API.createTextLabel(string.Format("~g~${0}~w~, Level {1}", propriedades[index].ValorPropriedade, propriedades[index].Level),
                                new Vector3(propriedades[index].EntradaFrente.X, propriedades[index].EntradaFrente.Y, propriedades[index].EntradaFrente.Z - 0.150), 30, (float)0.3);
                        }
                    }
                    break;
            }
        }
    }
    #endregion

    #region Veículos
    private void CarregarVeiculos()
    {
        var cmd = new MySqlCommand("SELECT * FROM veiculos WHERE Spawnado ='1'", bancodados);
        var dr = cmd.ExecuteReader();
        int counter = 0;
        while (dr.Read())
        {
            SpawnarVeiculo(dr, null, 0);
            counter++;

            API.consoleOutput("[Spawnado] Vehicle ID: {0}", isInt(dr["ID"].ToString()));
        }
        dr.Close();

        API.consoleOutput("Veículos: {0} | Spawnados: {1}", veiculos.Count, counter);
    }

    private void CriarVeiculo(Client player, VehicleHash veiculo, int cor1, int cor2, Vector3 posicao, Vector3 rotacao, int dimensao, int proprietario = 0)
    {
        var veh = API.createVehicle(veiculo, posicao, rotacao, 0, 0, dimensao);
        veh.engineStatus = false;
        //; API.setVehicleCustomPrimaryColor(veh, cor1.red, cor1.green, cor1.blue);
        //; API.setVehicleCustomSecondaryColor(veh, cor2.red, cor2.green, cor2.blue);

        if (player != null) API.setPlayerIntoVehicle(player, veh, -1);

        var v = new Veiculo();
        v.Modelo = veiculo.ToString();
        //; v.Cor1 = cor1;
        //; v.Cor2 = cor2;
        v.Posicao = API.getEntityPosition(veh);
        v.Rotacao = API.getEntityRotation(veh);
        v.Veh = veh;
        v.Placa = gerarPlacaVeiculo();
        v.IDPersonagemProprietario = proprietario;
        API.setVehicleNumberPlate(veh, v.Placa);
        v.Explodido = 0;
        v.gasolina = 80;

        if (proprietario != 0)
            API.setVehicleLocked(veh, true);

        var str = string.Format(@"INSERT INTO veiculos (ModeloVeiculo, PosX, PosZ, PosY, RotX, RotZ, RotY, Cor1R, Cor1G, Cor1B, Cor2R, Cor2G, Cor2B, Placa, Dimensao, IDPersonagemProprietario) 
                VALUES ({0}, 
                {1}, {2}, {3}, 
                {4}, {5}, {6}, 
                {7}, {8}, {9}, 
                {10}, {11}, {12},
                {13}, {14}, {15})",
            at(veiculo.ToString()),
           aFlt(v.Posicao.X),
           aFlt(v.Posicao.Z),
           aFlt(v.Posicao.Y),
           aFlt(v.Rotacao.X),
           aFlt(v.Rotacao.Z),
           aFlt(v.Rotacao.Y),
            v.Cor1.red,
            v.Cor1.green,
            v.Cor1.blue,
            v.Cor2.red,
            v.Cor2.green,
            v.Cor2.blue,
            at(v.Placa),
            dimensao,
            proprietario
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        v.ID = id;
        API.setEntitySyncedData(veh, "id", id);

        veiculos.Add(v);
    }

    private void SpawnarVeiculo(MySqlDataReader dr, Client player, int desexplodir = 0)
    {
        var veh = new Veiculo();

        veh.Explodido = isInt(dr["explodido"].ToString());
        veh.Modelo = dr["ModeloVeiculo"].ToString();

        veh.IDFaccao = isInt(dr["IDFaccao"].ToString());

        if (veh.IDFaccao > 0)
            veh.Explodido = 0;

        if (veh.Explodido == 0 || desexplodir == 1)
        {
            if (desexplodir == 1)
            {
                veh.Explodido = 0;

                veh.Posicao = new Vector3(-239.193, -1160.458, 22.53535);
            }
            else
            {
                veh.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            }

            veh.ID = isInt(dr["ID"].ToString());
            veh.Placa = dr["Placa"].ToString();

            veh.PosicaoSpawn = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            veh.Rotacao = new Vector3(isFloat(dr["RotX"].ToString()), isFloat(dr["RotY"].ToString()), isFloat(dr["RotZ"].ToString()));

            veh.Cor1 = new GrandTheftMultiplayer.Server.Constant.Color(isInt(dr["Cor1R"].ToString()), isInt(dr["Cor1G"].ToString()), isInt(dr["Cor1B"].ToString()));
            veh.Cor2 = new GrandTheftMultiplayer.Server.Constant.Color(isInt(dr["Cor2R"].ToString()), isInt(dr["Cor2G"].ToString()), isInt(dr["Cor2B"].ToString()));

            veh.Vida = isFloat(dr["Vida"].ToString());
            veh.IDPersonagemProprietario = isInt(dr["IDPersonagemProprietario"].ToString());
            veh.gasolina = isFloat(dr["gasolina"].ToString());

            veh.Inv1 = dr["inv1"].ToString();
            veh.Inv2 = dr["inv2"].ToString();
            veh.Inv3 = dr["inv3"].ToString();
            veh.Inv4 = dr["inv4"].ToString();
            veh.Inv5 = dr["inv5"].ToString();
            veh.Inv6 = dr["inv6"].ToString();
            veh.Inv7 = dr["inv7"].ToString();
            veh.Inv8 = dr["inv8"].ToString();
            veh.Inv9 = dr["inv9"].ToString();
            veh.Inv10 = dr["inv10"].ToString();
            veh.Inv1_q = isInt(dr["inv1_q"].ToString());
            veh.Inv2_q = isInt(dr["inv2_q"].ToString());
            veh.Inv3_q = isInt(dr["inv3_q"].ToString());
            veh.Inv4_q = isInt(dr["inv4_q"].ToString());
            veh.Inv5_q = isInt(dr["inv5_q"].ToString());
            veh.Inv6_q = isInt(dr["inv6_q"].ToString());
            veh.Inv7_q = isInt(dr["inv7_q"].ToString());
            veh.Inv8_q = isInt(dr["inv8_q"].ToString());
            veh.Inv9_q = isInt(dr["inv9_q"].ToString());
            veh.Inv10_q = isInt(dr["inv10_q"].ToString());

            veh.Veh = API.createVehicle(API.vehicleNameToModel(veh.Modelo), veh.Posicao, veh.Rotacao, 0, 0);
            API.setVehicleHealth(veh.Veh, veh.Vida);
            API.setVehicleNumberPlate(veh.Veh, veh.Placa);
            API.setVehicleCustomPrimaryColor(veh.Veh, veh.Cor1.red, veh.Cor1.green, veh.Cor1.blue);
            API.setVehicleCustomSecondaryColor(veh.Veh, veh.Cor2.red, veh.Cor2.green, veh.Cor2.blue);
            if (veh.IDPersonagemProprietario > 0) API.setVehicleLocked(veh.Veh, true);
            veh.Veh.engineStatus = false;
            API.setEntitySyncedData(veh.Veh, "id", veh.ID);

            var fac = recuperarFaccaoPorID(veh.IDFaccao);
            if (fac != null)
            {
                if (fac.TipoFaccao.Equals(Faccao.Tipo.Policial))
                    API.setVehicleEnginePowerMultiplier(veh.Veh, 15);
            }

            if (player != null)
            {
                EnviarMensagemShard(player, string.Format("{0} SPAWNADO", veh.Modelo, 3));
            }

            veiculos.Add(veh);
        }
        else
        {
            if (player != null)
            {
                EnviarMensagemErro(player, "O seu " + veh.Modelo + " explodiu... Passe na Mors Mutual Seguros para recupera-lo.");
            }
        }
    }

    private void SalvarPortaMalas(int vehID)
    {
        var veh = recuperarVeiculoPorID(vehID);
        if (veh == null) return;

        var str_save_pm = string.Format("UPDATE veiculos SET inv1='{1}',inv2='{2}',inv3='{3}',inv4='{4}',inv5='{5}',inv6='{6}',inv7='{7}',inv8='{8}',inv9='{9}',inv10='{10}',inv1_q={11},inv2_q={12},inv3_q={13},inv4_q={14},inv5_q={15},inv6_q={16},inv7_q={17},inv8_q={18},inv9_q={19},inv10_q={20} WHERE ID={0}", vehID,
            veh.Inv1,
            veh.Inv2,
            veh.Inv3,
            veh.Inv4,
            veh.Inv5,
            veh.Inv6,
            veh.Inv7,
            veh.Inv8,
            veh.Inv9,
            veh.Inv10,
            veh.Inv1_q,
            veh.Inv2_q,
            veh.Inv3_q,
            veh.Inv4_q,
            veh.Inv5_q,
            veh.Inv6_q,
            veh.Inv7_q,
            veh.Inv8_q,
            veh.Inv9_q,
            veh.Inv10_q
            );
        var cmd_spm = new MySqlCommand(str_save_pm, bancodados);
        cmd_spm.ExecuteNonQuery();

    }

    private void MotorVeiculo(Client player, int status = 0)
    {
        if (!player.isInVehicle || player.vehicleSeat != -1)
        {
            EnviarMensagemErro(player, "Você não está no banco do motorista de um veículo.");
            return;
        }

        if (API.hasEntitySyncedData(player.handle, "CarJob"))
        {
            if (API.getPlayerVehicle(player) == API.getEntitySyncedData(player.handle, "CarJob"))
            {
                API.sendNotificationToPlayer(player, "~r~Você não pode ligar/desligar o motor deste veículo.");
                return;
            }
        }

        var veh = (Veiculo)recuperarVeiculoPorID(API.getEntitySyncedData(player.vehicle, "id"));
        if (veh.IDPersonagemProprietario == 0)
        {
            if (API.getEntitySyncedData(player, "idfaccao") != veh.IDFaccao)
            {
                EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                return;
            }
        }
        else
        {
            if (API.getEntitySyncedData(player, "id") != veh.IDPersonagemProprietario)
            {
                EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                return;
            }
        }

        if (API.hasEntitySyncedData(player.handle, "player_abastecendo"))
        {
            EnviarMensagemErro(player, "Você não pode alterar o status do motor em quanto abastece.");
            return;
        }

        if (status == 0)
            player.vehicle.engineStatus = !player.vehicle.engineStatus;
        else if (status == 1)//ligar motor
        {
            if (veh.gasolina <= 0)
            {
                EnviarMensagemErro(player, "O veículo está sem gasolina.");
                return;
            }
            if (player.vehicle.engineStatus == true)
            {
                EnviarMensagemErro(player, "O motor já está ligado.");
                return;
            }
            player.vehicle.engineStatus = true;
        }
        else
        {
            if (player.vehicle.engineStatus == false)
            {
                EnviarMensagemErro(player, "O motor já está desligado.");
                return;
            }
            player.vehicle.engineStatus = false;
        }

        if (player.vehicle.engineStatus)
            EnviarMensagemSubtitulo(player, "Motor ~g~ligado");
        else
            EnviarMensagemSubtitulo(player, "Motor ~r~desligado");
    }

    #endregion

    #region Armários
    private void CriarArmario(Client player, int faccao)
    {
        var str = string.Format(@"INSERT INTO armarios (PosX, PosY, PosZ, Dimensao, IDFaccao, IDRank) VALUES ({0}, {1}, {2}, {3}, {4}, 1)",
            player.position.X.ToString().Replace(",", "."),
            player.position.Y.ToString().Replace(",", "."),
            player.position.Z.ToString().Replace(",", "."),
            player.dimension,
            API.getEntitySyncedData(player, "idfaccao") + 1
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var arm = new Armario();
        arm.ID = id;
        arm.Posicao = player.position;
        arm.Dimensao = player.dimension;
        arm.IDFaccao = API.getEntitySyncedData(player, "idfaccao");
        arm.IDRank = 1;
        arm.Text = API.createTextLabel("[/armario]", arm.Posicao, 30, (float)0.4, false, arm.Dimensao);
        armarios.Add(arm);
    }

    private void CarregarArmarios()
    {
        var cmd = new MySqlCommand("SELECT * FROM armarios", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var arm = new Armario();
            arm.ID = isInt(dr["ID"].ToString());
            arm.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            arm.Dimensao = isInt(dr["Dimensao"].ToString());
            arm.IDFaccao = isInt(dr["IDFaccao"].ToString());
            arm.IDRank = isInt(dr["IDRank"].ToString());
            arm.Text = API.createTextLabel("[/armario]", arm.Posicao, 30, (float)0.4, false, arm.Dimensao);

            armarios.Add(arm);
        }
        dr.Close();

        // preciso percorrer dnv pois não aceita dois dr abertos simultaneos
        foreach (var arm in armarios)
        {
            var cmd2 = new MySqlCommand(string.Format("SELECT * FROM armariositens WHERE IDArmario = {0} ORDER BY Arma", arm.ID), bancodados);
            var dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                var item = new Armario.Item();
                item.Arma = dr2["Arma"].ToString();
                item.Municao = isInt(dr2["Municao"].ToString());
                item.Estoque = isInt(dr2["Estoque"].ToString());
                item.IDRank = isInt(dr2["IDRank"].ToString());
                item.Pintura = isInt(dr2["Pintura"].ToString());
                item.Componentes = dr2["Componentes"].ToString();
                arm.Itens.Add(item);
            }
            dr2.Close();
        }

        API.consoleOutput("Armários: {0}", armarios.Count);
    }

    private void PegarItemArmario(Client player, int id, string arma)
    {
        var armario = recuperarArmarioPorID(id);
        var item = recuperarArmarioItemPorArma(id, arma);

        var idarmario = armarios.IndexOf(armario);
        var iditem = armarios[idarmario].Itens.IndexOf(item);

        if (armarios[idarmario].Itens[iditem].Estoque.Equals(0))
        {
            EnviarMensagemErro(player, "O armário não possui estoque para o item selecionado.");
            return;
        }

        foreach (WeaponHash wepHash in API.getPlayerWeapons(player))
        {
            if (wepHash.ToString().Equals(arma))
            {
                if (API.getPlayerWeaponAmmo(player, wepHash) == 0)
                {
                    API.setPlayerWeaponAmmo(player, API.weaponNameToModel(arma), armarios[idarmario].Itens[iditem].Municao);
                    API.setPlayerWeaponTint(player, API.weaponNameToModel(arma), (WeaponTint)armarios[idarmario].Itens[iditem].Pintura);
                    armarios[idarmario].Itens[iditem].Estoque--;
                    var str2 = string.Format("UPDATE armariositens SET Estoque = {0} WHERE IDArmario = {1} AND Arma = {2}",
                         armarios[idarmario].Itens[iditem].Estoque,
                         id,
                         at(arma));
                    var cmd2 = new MySqlCommand(str2, bancodados);
                    cmd2.ExecuteNonQuery();
                    return;
                }
                else
                {
                    EnviarMensagemErro(player, "Você já possui o item selecionado.");
                    return;
                }
            }
        }

        API.givePlayerWeapon(player, API.weaponNameToModel(arma), armarios[idarmario].Itens[iditem].Municao, true, true);
        API.setPlayerWeaponTint(player, API.weaponNameToModel(arma), (WeaponTint)armarios[idarmario].Itens[iditem].Pintura);
        armarios[idarmario].Itens[iditem].Estoque--;

        var componentes = JsonConvert.DeserializeObject<List<WeaponComponent>>(armarios[idarmario].Itens[iditem].Componentes);
        foreach (var comp in componentes)
            API.givePlayerWeaponComponent(player, API.weaponNameToModel(arma), comp);

        var str = string.Format("UPDATE armariositens SET Estoque = {0} WHERE IDArmario = {1} AND Arma = {2}",
             armarios[idarmario].Itens[iditem].Estoque,
             id,
             at(arma));
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
    }
    #endregion



    #region Interações
    private void EntrarPropriedade(Client player, int id, int apt_menu_status)
    {
        if (player.isInVehicle)
        {
            EnviarMensagemErro(player, "Você não pode fazer isso estando em um veículo.");
            return;
        }

        var prop = recuperarPropriedadePorID(id);

        //=== Se a casa estiver a venda abrir o menu de compra
        if (prop.IDPersonagemProprietario == 0)
        {
            var param1 = new List<string>();
            var param2 = new List<string>();

            API.setEntitySyncedData(player, "Prop_Menu", prop.ID);

            param1.Add(string.Format("Comprar"));
            param2.Add(string.Format("Valor: {0} | Level: {1}", prop.ValorPropriedade, prop.Level));

            param1.Add(string.Format("Fechar menu"));
            param2.Add(string.Format("Não quero comprar essa casa."));

            API.triggerClientEvent(player, "menucomresposta", "COMPRAR CASA", "Listagem de Propriedades", 6, param1, param2);

            return;
        }

        var prop_id = prop.IDPropriedade; //Pra ver se é prédio
        if (prop_id != 0)
        {
            var prop2 = recuperarPropriedadePorID(prop_id);

            if (prop2.TipoPropriedade.Equals(Propriedade.Tipo.Predio) && apt_menu_status == 1)
            {
                if (prop.IDPersonagemProprietario == API.getEntitySyncedData(player, "id"))
                {
                    var param1 = new List<string>();
                    var param2 = new List<string>();

                    API.setEntitySyncedData(player, "Prop_Menu", prop.ID);

                    param1.Add(string.Format("Entrar"));
                    param2.Add(string.Format("Entrar na propriedade"));

                    if (!prop.StatusPortaFrente)
                        param1.Add(string.Format("Destrancar"));
                    else
                        param1.Add(string.Format("Trancar"));
                    param2.Add(string.Format("Trancar/Destrancar a propriedade"));

                    param1.Add(string.Format("Abandonar"));
                    param2.Add(string.Format("Abandonar a propriedade"));

                    param1.Add(string.Format("Fechar menu"));
                    param2.Add(string.Format("Fechar menu."));

                    API.triggerClientEvent(player, "menucomresposta", "APARTAMENTO", "Listagem de Propriedades", 6, param1, param2);

                    return;
                }
            }
        }

        //===

        if (prop.Interior == 0 || prop.SaidaFrente.X == 0)
        {
            EnviarMensagemErro(player, "A propriedade não possui um interior.");
            return;
        }

        if (prop.IDFaccao != 0)
        {
            if (prop.IDFaccao != API.getEntitySyncedData(player, "idfaccao"))
            {
                EnviarMensagemErro(player, "A porta da frente está trancada.");
                return;
            }
        }
        else
        {
            if (!prop.StatusPortaFrente)
            {
                EnviarMensagemErro(player, "A porta da frente está trancada.");
                return;
            }
        }

        var ipl = RecuperarIPLPorInterior(prop.Interior);
        if (ipl != "")
        {
            API.sendNativeToPlayer(player, Hash.REQUEST_IPL, ipl);
            API.setEntitySyncedData(player, "ipl", ipl);
        }

        API.setEntitySyncedData(player, "idpropriedade", prop.ID);
        API.setEntityDimension(player, prop.ID);
        API.setEntityPosition(player, prop.SaidaFrente);

        //Entrar com Cachorro
        if (API.getEntitySyncedData(player, "TemDog") == 1)
        {
            var pet = recuperarPetPorID(API.getEntitySyncedData(player, "Dog_ID"));
            pet.Dimensao = prop.ID;
            API.setEntityPosition(pet.Ped, prop.SaidaFrente);
        }

    }

    private void InteracaoEntrar(Client player)
    {
        foreach (var prop in propriedades)
        {
            var play = API.getPlayersInRadiusOfPosition(2, prop.EntradaFrente).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                if (prop.TipoPropriedade.Equals(Propriedade.Tipo.Predio))
                {
                    var param1 = new List<string>();
                    var param2 = new List<string>();
                    foreach (var p in propriedades.Where(pr => pr.IDPropriedade == prop.ID).ToList())
                    {
                        var str = "A Venda";
                        if (p.IDPersonagemProprietario != 0)
                            str = p.NomePersonagemProprietario;

                        param1.Add(string.Format("{0}, {1}", p.ID, str));
                        param2.Add(string.Format("Valor: {0} | Level: {1}", p.ValorPropriedade, p.Level));
                    }

                    API.triggerClientEvent(player, "menucomresposta", "PROPRIEDADES", "Listagem de Propriedades", 6, param1, param2);
                }
                else
                {
                    EntrarPropriedade(player, prop.ID, 1);
                }
                return;
            }
        }

        EnviarMensagemErro(player, "Você não está próximo a nenhuma propriedade.");
    }

    private void InteracaoSair(Client player)
    {
        if (player.isInVehicle)
        {
            EnviarMensagemErro(player, "Você não pode fazer isso estando em um veículo.");
            return;
        }

        var prop = recuperarPropriedadePorID(API.getEntitySyncedData(player, "idpropriedade"));
        if (prop == null) return;

        var play = API.getPlayersInRadiusOfPosition(2, (Vector3)prop.SaidaFrente).Where(p => p.name.Equals(player.name)).ToList();
        if (play.Count > 0)
        {
            if (prop.IDFaccao != 0)
            {
                if (prop.IDFaccao != API.getEntitySyncedData(player, "idfaccao"))
                {
                    EnviarMensagemErro(player, "A porta da frente está trancada.");
                    return;
                }
            }
            else
            {
                if (!prop.StatusPortaFrente)
                {
                    EnviarMensagemErro(player, "A porta da frente está trancada.");
                    return;
                }
            }

            API.setEntitySyncedData(player, "idpropriedade", 0);
            API.setEntityDimension(player, 0);

            if (API.getEntitySyncedData(player, "ipl") != "")
                API.sendNativeToPlayer(player, Hash.REMOVE_IPL, API.getEntitySyncedData(player, "ipl"));

            API.setEntitySyncedData(player, "ipl", string.Empty);

            var pos = new Vector3();
            if (prop.IDPropriedade == 0)
            {
                pos = prop.EntradaFrente;
            }
            else
            {
                var prop2 = recuperarPropriedadePorID(prop.IDPropriedade);
                pos = prop2.EntradaFrente;
            }

            API.setEntityPosition(player, pos);

            //Sair com Cachorro
            if (API.getEntitySyncedData(player, "TemDog") == 1)
            {
                var pet = recuperarPetPorID(API.getEntitySyncedData(player, "Dog_ID"));
                pet.Dimensao = 0;
                API.setEntityPosition(pet.Ped, pos);
            }
        }
    }

    private void InteracaoAbrirTrancar(Client player)
    {

        if (API.hasEntitySyncedData(player.handle, "CarJob"))
        {
            if (API.getPlayerVehicle(player) == API.getEntitySyncedData(player.handle, "CarJob"))
            {
                API.sendNotificationToPlayer(player, "~r~Você não pode trancar/destrancar este veículo.");
                return;
            }
        }

        foreach (var veh in API.getAllVehicles())
        {
            var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                if (API.getEntitySyncedData(player, "id") != recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id")).IDPersonagemProprietario)
                {
                    EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                    return;
                }

                API.setVehicleLocked(veh, !API.getVehicleLocked(veh));
                if (API.getVehicleLocked(veh))
                    EnviarMensagemSubtitulo(player, "Veículo ~r~trancado");
                else
                    EnviarMensagemSubtitulo(player, "Veículo ~g~aberto");
                return;
            }
        }
    }

    private void InteracaoAbrirTrancarPortaMalas(Client player)
    {
        foreach (var veh in API.getAllVehicles())
        {
            var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                var v = (Veiculo)recuperarVeiculoPorID(API.getEntitySyncedData(player.vehicle, "id"));
                if (v.IDPersonagemProprietario == 0)
                {
                    if (API.getEntitySyncedData(player, "idfaccao") != v.IDFaccao)
                    {
                        EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                        return;
                    }
                }
                else
                {
                    if (API.getEntitySyncedData(player, "id") != v.IDPersonagemProprietario)
                    {
                        EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                        return;
                    }
                }

                API.setVehicleDoorState(veh, 5, !API.getVehicleDoorState(veh, 5));
                if (!API.getVehicleDoorState(veh, 5))
                    EnviarMensagemSubtitulo(player, "Porta-malas ~r~fechado");
                else
                    EnviarMensagemSubtitulo(player, "Porta-malas ~g~aberto");
                return;
            }
        }
    }

    private void InteracaoAbrirTrancarCapo(Client player)
    {
        foreach (var veh in API.getAllVehicles())
        {
            var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                var v = (Veiculo)recuperarVeiculoPorID(API.getEntitySyncedData(player.vehicle, "id"));
                if (v.IDPersonagemProprietario == 0)
                {
                    if (API.getEntitySyncedData(player, "idfaccao") != v.IDFaccao)
                    {
                        EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                        return;
                    }
                }
                else
                {
                    if (API.getEntitySyncedData(player, "id") != v.IDPersonagemProprietario)
                    {
                        EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                        return;
                    }
                }

                API.setVehicleDoorState(veh, 4, !API.getVehicleDoorState(veh, 4));
                if (!API.getVehicleDoorState(veh, 4))
                    EnviarMensagemSubtitulo(player, "Capô ~r~fechado");
                else
                    EnviarMensagemSubtitulo(player, "Capô ~g~aberto");
                return;
            }
        }
    }

    private void InteracaoAbrirTrancarPortaVeh(Client player, int porta)
    {
        foreach (var veh in API.getAllVehicles())
        {
            var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                var v = (Veiculo)recuperarVeiculoPorID(API.getEntitySyncedData(player.vehicle, "id"));
                if (v.IDPersonagemProprietario == 0)
                {
                    if (API.getEntitySyncedData(player, "idfaccao") != v.IDFaccao)
                    {
                        EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                        return;
                    }
                }
                else
                {
                    if (API.getEntitySyncedData(player, "id") != v.IDPersonagemProprietario)
                    {
                        EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                        return;
                    }
                }

                API.setVehicleDoorState(veh, porta, !API.getVehicleDoorState(veh, porta));
                if (!API.getVehicleDoorState(veh, porta))
                    EnviarMensagemSubtitulo(player, string.Format("Porta {0} ~r~fechada", porta + 1));
                else
                    EnviarMensagemSubtitulo(player, string.Format("Porta {0} ~g~aberta", porta + 1));
                return;
            }
        }
    }
    #endregion

    #region Mensagens
    private void EnviarMensagemShard(Client player, string msg, int segundos = 5)
    {
        API.triggerClientEvent(player, "showShard", msg, segundos * 1000);
    }

    private void EnviarMensagemSubtitulo(Client player, string msg, int segundos = 3)
    {
        API.triggerClientEvent(player, "showSubtitle", msg, segundos * 1000);
    }

    private void EnviarMensagemEntradaSaida(Client player, string tipo)
    {
        var msg = string.Format("{0} {1} {2} servidor.",
            player.name,
            tipo,
            tipo.Equals("entrou") ? "no" : "do");
        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado"))
                if (!API.getEntitySyncedData(target, "togentradasaida"))
                    API.sendNotificationToPlayer(target, msg);
        }
    }

    private void EnviarMensagemChatOOCLocal(Client player, string msg)
    {
        msg = string.Format("(( {0} [{1}]: {2} ))", player.name, recuperarIDPorClient(player), msg);
        var playersProximos = API.getPlayersInRadiusOfPlayer(DISTANCIA_RP, player);
        foreach (var target in playersProximos)
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntityDimension(target) == API.getEntityDimension(player))
                if (!API.getEntitySyncedData(target, "togchatooclocal"))
                    target.sendChatMessage(COR_CHAT_OOC_LOCAL, msg);
        }
    }

    private void EnviarMensagemRadius(Client player, float radius, string msg, string cor = "")
    {
        var playersProximos = API.getPlayersInRadiusOfPlayer(radius, player);
        var corPlayer = "~#FFFFFF~";
        var corTarget = "~#B5B5B5~";

        if (cor != "")
            corPlayer = corTarget = cor;

        foreach (var target in playersProximos)
        {
            if (API.hasEntitySyncedData(target, "logado") && API.getEntityDimension(target) == API.getEntityDimension(player))
            {
                if (player == target)
                    target.sendChatMessage(corPlayer, msg);
                else
                    target.sendChatMessage(corTarget, msg);
            }
        }
    }

    private void EnviarMensagemVeiculo(Client player, string msg)
    {
        var corPlayer = COR_PM_ENVIADA;
        var corTarget = COR_PM_RECEBIDA;

        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntityDimension(target) == API.getEntityDimension(player)
                && player.vehicle == target.vehicle)
            {
                if (player == target)
                    target.sendChatMessage(corPlayer, msg);
                else
                    target.sendChatMessage(corTarget, msg);
            }
        }
    }

    private void EnviarMensagemChatStaff(string msg)
    {
        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntitySyncedData(target, "staff") >= 2)
                if (!API.getEntitySyncedData(target, "togchatstaff"))
                    target.sendChatMessage(COR_STAFF, msg);
        }
    }

    private void EnviarMensagemStaff(int staff, string msg)
    {
        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntitySyncedData(target, "staff") >= staff
                && !API.getEntitySyncedData(target, "togstaff"))
                API.sendNotificationToPlayer(target, "~y~" + msg);
        }
    }

    private void EnviarMensagemSOS(Client player, string msg)
    {
        var sos = recuperarSOSPorID(recuperarIDPorClient(player));
        if (sos != null)
        {
            EnviarMensagemErro(player, "Você já possui um SOS enviado aguardando atendimento. Pedimos sua compreensão.");
            return;
        }

        sos = new SOS();
        sos.IDPlayer = recuperarIDPorClient(player);
        sos.NomePersonagem = player.name;
        sos.Descricao = msg;
        listaSOS.Add(sos);

        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado") && API.getEntitySyncedData(target, "staff") > 0)
            {
                API.sendChatMessageToPlayer(target, "~#90C3D4~",
                    string.Format("SOS de {1} [{0}] [/aj {0} ou /rj {0}]", sos.IDPlayer, player.name));
                API.sendChatMessageToPlayer(target, msg);
            }
        }

        EnviarMensagemSucesso(player, "SOS enviado com sucesso. Sua posição na fila é: " + listaSOS.Count);
    }

    private void EnviarMensagemErro(Client player, string msg)
    {
        API.sendNotificationToPlayer(player, "~r~[ERRO] ~w~" + msg);
    }

    private void EnviarMensagemSucesso(Client player, string msg)
    {
        API.sendNotificationToPlayer(player, msg);
    }

    private void EnviarMensagemChatOOCFaccao(Client player, string msg)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        var rank = recuperarFaccaoRankPorID(API.getEntitySyncedData(player, "idfaccao"), API.getEntitySyncedData(player, "idrank"));

        msg = string.Format("(( {0} {1} [{2}]: {3} ))",
            rank.NomeRank,
            player.name,
            recuperarIDPorClient(player), msg);
        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntitySyncedData(target, "idfaccao") == API.getEntitySyncedData(player, "idfaccao"))
                if (!API.getEntitySyncedData(target, "togchatfaccao"))
                    target.sendChatMessage(string.Format("~#{0}~", faccao.CorFaccao), msg);
        }
    }

    private void EnviarMensagemRadio(Client player, int slot, int canal, string msg)
    {
        var targetsProxs = API.getPlayersInRadiusOfPlayer(DISTANCIA_RP, player);
        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && (API.getEntitySyncedData(target, "canalradio") == canal
                    || API.getEntitySyncedData(target, "canalradio2") == canal
                    || API.getEntitySyncedData(target, "canalradio3") == canal))
            {
                API.sendChatMessageToPlayer(target, COR_RADIO, string.Format("[S:{0} C:{1}] {2}: {3}",
                    recuperarSlotRadio(target, canal),
                    canal,
                    player.name,
                    msg));

                if (targetsProxs.Where(p => p.name.Equals(target.name) && !p.name.Equals(player.name)).ToList().Count > 0)
                    API.sendChatMessageToPlayer(target, string.Format("{0} diz no rádio: {1}", player.name, msg));
            }
        }
    }

    private void EnviarMensagemOOCFaccao(int faccao, string msg)
    {
        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntitySyncedData(target, "idfaccao") == faccao)
                if (!API.getEntitySyncedData(target, "togfaccao"))
                    API.sendNotificationToPlayer(target, msg);
        }
    }

    private void EnviarMensagemFaccao(int faccao, string msg)
    {
        var fac = recuperarFaccaoPorID(faccao);

        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado")
                && API.getEntitySyncedData(target, "idfaccao") == faccao)
                if (!API.getEntitySyncedData(target, "togfaccao"))
                    API.sendChatMessageToPlayer(target, string.Format("~#{0}~", fac.CorFaccao), msg);
        }
    }

    private void EnviarPMStaff(Client player, Client target, string msg)
    {
        API.sendChatMessageToPlayer(player, COR_PM_STAFF, string.Format("(( PM para {0} [{1}]: {2} ))", target.name.Replace("_", " "), recuperarIDPorClient(target), msg));
        API.sendChatMessageToPlayer(target, COR_PM_STAFF, string.Format("(( PM de {0} [{1}]: {2} ))", player.name, recuperarIDPorClient(player), msg));
    }
    #endregion

    #region Personagens
    private void CarregarPersonagem(Client player, string nomePersonagem)
    {
        var str = string.Format(@"SELECT personagens.*
                        FROM personagens 
                        WHERE personagens.NomePersonagem = {0}", at(nomePersonagem));
        var cmd = new MySqlCommand(str, bancodados);
        var dr = cmd.ExecuteReader();

        if (dr.HasRows)
        {
            dr.Read();

            API.triggerClientEvent(player, "limparcamera");

            API.setWorldSyncedData("horario", string.Format("{0}:{1}", DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0')));

            API.setPlayerName(player, nomePersonagem.Replace("_", " "));
            API.setPlayerNametag(player, string.Format("{0} ({1})", nomePersonagem.Replace("_", " "), API.getEntitySyncedData(player, "idplayer")));

            API.setEntityTransparency(player, 255);
            API.freezePlayer(player, false);
            API.setEntityInvincible(player, false);

            // Carregar as informações do personagem
            player.setSkin(API.pedNameToModel(dr["Skin"].ToString()));
            API.setEntityDimension(player, isInt(dr["Dimensao"].ToString()));
            API.setPlayerHealth(player, isInt(dr["Vida"].ToString()));
            API.setPlayerArmor(player, isInt(dr["Colete"].ToString()));

            API.setEntitySyncedData(player, "ipl", dr["IPL"].ToString());
            if (dr["IPL"].ToString() != "")
                API.sendNativeToPlayer(player, Hash.REQUEST_IPL, dr["IPL"].ToString());

            var pos = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosZ"].ToString()), isFloat(dr["PosY"].ToString()));
            API.setEntityPosition(player, pos);

            var rot = new Vector3(isFloat(dr["RotX"].ToString()), isFloat(dr["RotY"].ToString()), isFloat(dr["RotZ"].ToString()));
            API.setEntityRotation(player, rot);

            API.setEntitySyncedData(player, "logado", true);
            API.setEntitySyncedData(player, "id", isInt(dr["ID"].ToString()));

            API.setEntitySyncedData(player, "datahoraultimoacesso", dr["DataHoraUltimoAcesso"].ToString());
            API.setEntitySyncedData(player, "ipultimoacesso", dr["IPUltimoAcesso"].ToString());
            API.setEntitySyncedData(player, "idfaccao", isInt(dr["IDFaccao"].ToString()));
            API.setEntitySyncedData(player, "idrank", isInt(dr["IDRank"].ToString()));
            API.setEntitySyncedData(player, "level", isInt(dr["Level"].ToString()));
            API.setEntitySyncedData(player, "xp", isInt(dr["XP"].ToString()));
            API.setEntitySyncedData(player, "tempoconectado", isInt(dr["TempoConectado"].ToString()));
            API.setEntitySyncedData(player, "dinheiro", isInt(dr["Dinheiro"].ToString()));
            API.setEntitySyncedData(player, "canalradio", isInt(dr["CanalRadio"].ToString()));
            API.setEntitySyncedData(player, "canalradio2", isInt(dr["CanalRadio2"].ToString()));
            API.setEntitySyncedData(player, "canalradio3", isInt(dr["CanalRadio3"].ToString()));
            API.setEntitySyncedData(player, "idmascara", isInt(dr["IDMascara"].ToString()));
            API.setEntitySyncedData(player, "banco", isInt(dr["Banco"].ToString()));
            API.setEntitySyncedData(player, "skin", dr["Skin"].ToString());
            API.setEntitySyncedData(player, "skin2", dr["Skin2"].ToString());
            API.setEntitySyncedData(player, "Job", isInt(dr["emprego"].ToString()));
            API.setEntitySyncedData(player, "carLic", isInt(dr["carLic"].ToString()));

            API.setEntitySyncedData(player, "ferido", false);
            API.setEntitySyncedData(player, "morto", false);
            API.setEntitySyncedData(player, "datasalvamento", DateTime.Now.ToString());
            API.setEntitySyncedData(player, "idpropriedade", isInt(dr["Dimensao"].ToString()));
            API.setEntitySyncedData(player, "tipoconvite", 0);
            API.setEntitySyncedData(player, "idconvite", 0);
            API.setEntitySyncedData(player, "idconvidador", 0);
            API.setEntitySyncedData(player, "atrabalho", false);
            API.setEntitySyncedData(player, "trabalho", false);
            API.setEntitySyncedData(player, "algemado", false);

            // Togs
            API.setEntitySyncedData(player, "togchatooclocal", false);
            API.setEntitySyncedData(player, "togchatstaff", false);
            API.setEntitySyncedData(player, "togstaff", false);
            API.setEntitySyncedData(player, "togchatfaccao", false);
            API.setEntitySyncedData(player, "togfaccao", false);
            API.setEntitySyncedData(player, "togentradasaida", true);
            API.setEntitySyncedData(player, "togpm", false);
            API.setEntitySyncedData(player, "toghud", false);

            dr.Close();

            string str_inv = string.Format(@"SELECT * FROM personagensinventario WHERE IDPersonagem = {0}", API.getEntitySyncedData(player, "id"));
            cmd = new MySqlCommand(str_inv, bancodados);
            var dritens = cmd.ExecuteReader();

            int NaoTemInv = 0;

            if (dritens.HasRows)
            {
                dritens.Read();

                API.setEntitySyncedData(player, "inv1", dritens["s1"].ToString());
                API.setEntitySyncedData(player, "inv2", dritens["s2"].ToString());
                API.setEntitySyncedData(player, "inv3", dritens["s3"].ToString());
                API.setEntitySyncedData(player, "inv4", dritens["s4"].ToString());
                API.setEntitySyncedData(player, "inv5", dritens["s5"].ToString());
                API.setEntitySyncedData(player, "inv6", dritens["s6"].ToString());
                API.setEntitySyncedData(player, "inv7", dritens["s7"].ToString());
                API.setEntitySyncedData(player, "inv8", dritens["s8"].ToString());
                API.setEntitySyncedData(player, "inv9", dritens["s9"].ToString());
                API.setEntitySyncedData(player, "inv10", dritens["s10"].ToString());

                API.setEntitySyncedData(player, "inv1q", isInt(dritens["q1"].ToString()));
                API.setEntitySyncedData(player, "inv2q", isInt(dritens["q2"].ToString()));
                API.setEntitySyncedData(player, "inv3q", isInt(dritens["q3"].ToString()));
                API.setEntitySyncedData(player, "inv4q", isInt(dritens["q4"].ToString()));
                API.setEntitySyncedData(player, "inv5q", isInt(dritens["q5"].ToString()));
                API.setEntitySyncedData(player, "inv6q", isInt(dritens["q6"].ToString()));
                API.setEntitySyncedData(player, "inv7q", isInt(dritens["q7"].ToString()));
                API.setEntitySyncedData(player, "inv8q", isInt(dritens["q8"].ToString()));
                API.setEntitySyncedData(player, "inv9q", isInt(dritens["q9"].ToString()));
                API.setEntitySyncedData(player, "inv10q", isInt(dritens["q10"].ToString()));
            }
            else
            {
                NaoTemInv = 1;
                API.setEntitySyncedData(player, "inv1", "vazio");
                API.setEntitySyncedData(player, "inv2", "vazio");
                API.setEntitySyncedData(player, "inv3", "vazio");
                API.setEntitySyncedData(player, "inv4", "vazio");
                API.setEntitySyncedData(player, "inv5", "vazio");
                API.setEntitySyncedData(player, "inv6", "vazio");
                API.setEntitySyncedData(player, "inv7", "vazio");
                API.setEntitySyncedData(player, "inv8", "vazio");
                API.setEntitySyncedData(player, "inv9", "vazio");
                API.setEntitySyncedData(player, "inv10", "vazio");

                API.setEntitySyncedData(player, "inv1q", 0);
                API.setEntitySyncedData(player, "inv2q", 0);
                API.setEntitySyncedData(player, "inv3q", 0);
                API.setEntitySyncedData(player, "inv4q", 0);
                API.setEntitySyncedData(player, "inv5q", 0);
                API.setEntitySyncedData(player, "inv6q", 0);
                API.setEntitySyncedData(player, "inv7q", 0);
                API.setEntitySyncedData(player, "inv8q", 0);
                API.setEntitySyncedData(player, "inv9q", 0);
                API.setEntitySyncedData(player, "inv10q", 0);
            }
            dritens.Close();

            if (NaoTemInv == 1)
            {
                string str_i = string.Format("INSERT INTO personagensinventario (IDPersonagem) VALUES ({0})", API.getEntitySyncedData(player, "id"));
                cmd = new MySqlCommand(str_i, bancodados);
                cmd.ExecuteNonQuery();

            }

            cmd = new MySqlCommand("SELECT * FROM personagensarmas WHERE IDPersonagem = " + API.getEntitySyncedData(player, "id"), bancodados);
            var dr2 = cmd.ExecuteReader();
            while (dr2.Read())
            {
                API.givePlayerWeapon(player, API.weaponNameToModel(dr2["Arma"].ToString()), isInt(dr2["Municao"].ToString()), false, true);
                API.setPlayerWeaponTint(player, API.weaponNameToModel(dr2["Arma"].ToString()), (WeaponTint)isInt(dr2["Pintura"].ToString()));

                var componentes = JsonConvert.DeserializeObject<List<WeaponComponent>>(dr2["Componentes"].ToString());
                foreach (var comp in componentes) API.givePlayerWeaponComponent(player, API.weaponNameToModel(dr2["Arma"].ToString()), comp);
            }
            dr2.Close();

            var clothes = new List<string>();
            cmd = new MySqlCommand(string.Format("SELECT * FROM personagensclothes WHERE IDPersonagem={0}", API.getEntitySyncedData(player, "id")), bancodados);
            dr2 = cmd.ExecuteReader();
            while (dr2.Read())
            {
                if (dr2["Skin"].ToString() == API.getEntitySyncedData(player, "skin"))
                    API.setPlayerClothes(player, isInt(dr2["Component"].ToString()), isInt(dr2["Drawable"].ToString()), isInt(dr2["Texture"].ToString()));
                else
                    clothes.Add(string.Format("{0}|{1}|{2}", dr2["Component"].ToString(), dr2["Drawable"].ToString(), dr2["Texture"].ToString()));
            }
            dr2.Close();
            API.setEntitySyncedData(player, "clothes", clothes);

            str = string.Empty;
            for (var i = 0; i <= 11; i++)
            {
                str += string.Format(" ({0}, {1}, {2}, {3}),",
                API.getEntitySyncedData(player, "id"),
                i,
                API.getPlayerClothesDrawable(player, i),
                API.getPlayerClothesTexture(player, i));
            }

            var accessorys = new List<string>();
            cmd = new MySqlCommand(string.Format("SELECT * FROM personagensaccessorys WHERE IDPersonagem={0}", API.getEntitySyncedData(player, "id")), bancodados);
            dr2 = cmd.ExecuteReader();
            while (dr2.Read())
            {
                if (dr2["Skin"].ToString() == API.getEntitySyncedData(player, "skin"))
                    API.setPlayerAccessory(player, isInt(dr2["Component"].ToString()), isInt(dr2["Drawable"].ToString()), isInt(dr2["Texture"].ToString()));
                else
                    accessorys.Add(string.Format("{0}|{1}|{2}", dr2["Component"].ToString(), dr2["Drawable"].ToString(), dr2["Texture"].ToString()));
            }
            dr2.Close();
            API.setEntitySyncedData(player, "accessorys", accessorys);

            str = string.Empty;
            for (var i = 0; i <= 11; i++)
            {
                str += string.Format(" ({0}, {1}, {2}, {3}),",
                API.getEntitySyncedData(player, "id"),
                i,
                API.getPlayerClothesDrawable(player, i),
                API.getPlayerClothesTexture(player, i));
            }

            // Salva informações de ultimo acesso
            str = string.Format("UPDATE personagens SET DataHoraUltimoAcesso = now(), IPUltimoAcesso = {0}, Online=1 WHERE ID = {1}", at(player.address), API.getEntitySyncedData(player, "id"));
            cmd = new MySqlCommand(str, bancodados);
            cmd.ExecuteNonQuery();

            // alguns natives
            foreach (var veh in API.getAllVehicles())
            {
                if (API.hasEntitySyncedData(veh, "elm"))
                    API.sendNativeToAllPlayers(0xD8050E0EB60CF274, veh, true);
                else
                    API.sendNativeToAllPlayers(0xD8050E0EB60CF274, veh, false);
            }

            EnviarMensagemEntradaSaida(player, "entrou");
            CriarLog(LOG_ENTRADAS, string.Format("{0} ({1}) entrou no servidor", player.name, player.address));

            LoadCustomPlayer(player);
        }
    }

    private void SalvarPersonagem(Client player)
    {
        if (!API.hasEntitySyncedData(player, "id") || !API.hasEntitySyncedData(player, "logado"))
            return;

        var str = string.Format(@"UPDATE personagens SET Vida={1}, Colete={2}, PosX={3}, PosY={4}, PosZ={5}, RotX={6}, RotY={7}, RotZ={8}, Skin={9}, IDFaccao={10}, IDRank={11},
            Level={12}, XP={13}, TempoConectado={14}, Dinheiro={15}, CanalRadio={16}, CanalRadio2={17}, CanalRadio3={18}, Banco={19}, Dimensao={20}, IPL={21}, Skin2={22}, emprego={23}, carLic={24}
            WHERE ID = {0}", API.getEntitySyncedData(player, "id"), player.health, player.armor,
            aFlt(player.position.X), aFlt(player.position.Z), aFlt(player.position.Y),
            aFlt(player.rotation.X), aFlt(player.rotation.Z), aFlt(player.rotation.Y), at(((PedHash)player.model).ToString()),
            API.getEntitySyncedData(player, "idfaccao"), API.getEntitySyncedData(player, "idrank"),
            API.getEntitySyncedData(player, "level"), API.getEntitySyncedData(player, "xp"), API.getEntitySyncedData(player, "tempoconectado"),
            API.getEntitySyncedData(player, "dinheiro"),
            API.getEntitySyncedData(player, "canalradio"), API.getEntitySyncedData(player, "canalradio2"), API.getEntitySyncedData(player, "canalradio3"),
            API.getEntitySyncedData(player, "banco"), API.getEntityDimension(player), at(API.getEntitySyncedData(player, "ipl")), at(API.getEntitySyncedData(player, "skin2")), API.getEntitySyncedData(player, "Job"), API.getEntitySyncedData(player, "carLic"));
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

        SaveCustomPlayer(player);

        cmd = new MySqlCommand(string.Format("DELETE FROM personagensclothes WHERE IDPersonagem = {0}; DELETE FROM personagensaccessorys WHERE IDPersonagem = {0}; DELETE FROM personagensarmas WHERE IDPersonagem = {0};",
            API.getEntitySyncedData(player, "id")), bancodados);
        cmd.ExecuteNonQuery();

        str = string.Empty;
        for (var i = 0; i <= 11; i++)
        {
            if (API.getPlayerClothesDrawable(player, i) == 0) continue;

            str += string.Format(" ({0}, {1}, {2}, {3}, {4}),",
            API.getEntitySyncedData(player, "id"),
             at(((PedHash)player.model).ToString()),
            i,
            API.getPlayerClothesDrawable(player, i),
            API.getPlayerClothesTexture(player, i));
        }

        var clothes = (List<object>)API.getEntitySyncedData(player, "clothes");
        foreach (var c in clothes)
            str += string.Format(" ({0}, {1}, {2}, {3}, {4}),",
            API.getEntitySyncedData(player, "id"),
             at(API.getEntitySyncedData(player, "skin2")),
            isInt(c.ToString().Split('|')[0]),
            isInt(c.ToString().Split('|')[1]),
            isInt(c.ToString().Split('|')[2]));

        if (str != string.Empty)
        {
            str = string.Format("INSERT INTO personagensclothes (IDPersonagem, Skin, Component, Drawable, Texture) VALUES {0}", str.Substring(0, str.Length - 1));
            cmd = new MySqlCommand(str, bancodados);
            cmd.ExecuteNonQuery();
        }

        // Está bugado, sofrendo para resolver
        /*str = string.Empty;
        for (var x = 0; x <= 2; x++)
        {
            str += string.Format(" ({0}, {1}, {2}, {3}),",
            API.getEntitySyncedData(player, "id"),
            x,
            API.getPlayerAccessoryDrawable(player, x),
            API.getPlayerAccessoryTexture(player, x));
        }

        if (str != string.Empty)
        {
            str = string.Format("INSERT INTO personagensaccessorys (IDPersonagem, Component, Drawable, Texture) VALUES {0}", str.Substring(0, str.Length - 1));
            API.consoleOutput(str);
            cmd = new MySqlCommand(str, bancodados);
            cmd.ExecuteNonQuery();
        }*/
        //}

        str = string.Format(@"UPDATE usuarios SET Staff={1}, SocialClub={2}, QuantidadeSOS={3}, TempoATrabalho={4} WHERE ID = {0}",
            API.getEntitySyncedData(player, "idusuario"),
            API.getEntitySyncedData(player, "staff"),
            at(player.socialClubName),
            API.getEntitySyncedData(player, "quantidadesos"),
            API.getEntitySyncedData(player, "tempoatrabalho"));
        cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

        var strArmas = string.Empty;
        foreach (var arma in API.getPlayerWeapons(player))
        {
            strArmas += string.Format(" ({0}, {1}, {2}, {3}, {4}),",
                API.getEntitySyncedData(player, "id"),
                at(arma.ToString()),
                API.getPlayerWeaponAmmo(player, arma),
                recuperarPinturaArma(API.getPlayerWeaponTint(player, arma).ToString()),
                at(JsonConvert.SerializeObject(API.getPlayerWeaponComponents(player, arma)))
                );
        }

        if (!strArmas.Equals(string.Empty))
        {
            strArmas = string.Format("INSERT INTO personagensarmas (IDPersonagem, Arma, Municao, Pintura, Componentes) VALUES {0}", strArmas.Substring(0, strArmas.Length - 1));
            cmd = new MySqlCommand(strArmas, bancodados);
            cmd.ExecuteNonQuery();
        }
    }
    #endregion

    #region Banimentos
    private void AdicionarBanimento(Client player, Client target, string motivo, int duracao = 0)
    {
        var str = string.Format(@"INSERT INTO banimentos (DataHoraBanimento, DataExpiracao, IDUsuarioBanido, IPBanido, SocialClub, MotivoBanimento, IDUsuarioStaff) 
        VALUES (now(), {0}, {1}, {2}, {3}, {4}, {5})",
            duracao == 0 ? "null" : at(DateTime.Now.AddHours(duracao).ToString("yyyy-MM-dd HH:mm:ss")),
            API.getEntitySyncedData(target, "id"),
            at(target.address),
            at(target.socialClubName),
            at(motivo),
            API.getEntitySyncedData(player, "id"));
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
    }

    private void AdicionarPunicao(Client player, Client target, string tipo, string motivo, int duracao = 0)
    {
        var str = "INSERT INTO punicoesadministrativas (TipoPunicao, DuracaoPunicao, DataHoraPunicao, IDPersonagemPunido, MotivoPunicao, IDUsuarioStaff) VALUES ";
        if (tipo.Equals("kick"))
            str += string.Format(" (1, 0, now(), {0}, {1}, {2})", API.getEntitySyncedData(target, "id"), at(motivo), API.getEntitySyncedData(player, "id"));
        else if (tipo.Equals("ban"))
            str += string.Format(" (2, 0, now(), {0}, {1}, {2})", API.getEntitySyncedData(target, "id"), at(motivo), API.getEntitySyncedData(player, "id"));
        else if (tipo.Equals("bantemp"))
            str += string.Format(" (2, {3}, now(), {0}, {1}, {2})", API.getEntitySyncedData(target, "id"), at(motivo), API.getEntitySyncedData(player, "id"), duracao);

        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
    }
    #endregion

    #region Blips
    private void CriarBlip(Client player, int tipo, int cor)
    {
        var str = string.Format(@"INSERT INTO blips (PosX, PosY, PosZ, Tipo, Cor) VALUES ({0}, {1}, {2}, {3}, {4})",
            player.position.X.ToString().Replace(",", "."),
            player.position.Y.ToString().Replace(",", "."),
            player.position.Z.ToString().Replace(",", "."),
            tipo,
            cor
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var blip = new adBlip();
        blip.ID = id;
        blip.Posicao = player.position;
        blip.Tipo = tipo;
        blip.Cor = cor;
        blip.Blip = API.createBlip(blip.Posicao);
        API.setBlipSprite(blip.Blip, tipo);
        API.setBlipColor(blip.Blip, cor);
        blips.Add(blip);

    }

    private void CarregarBlips()
    {
        var cmd = new MySqlCommand("SELECT * FROM blips", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var blip = new adBlip();
            blip.ID = isInt(dr["ID"].ToString());
            blip.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            blip.Cor = isInt(dr["Cor"].ToString());
            blip.Tipo = isInt(dr["Tipo"].ToString());
            blip.Nome = dr["Nome"].ToString();
            blip.Blip = API.createBlip(blip.Posicao);
            API.setBlipSprite(blip.Blip, blip.Tipo);
            API.setBlipColor(blip.Blip, blip.Cor);
            if (!blip.Nome.Equals(string.Empty)) API.setBlipName(blip.Blip, blip.Nome);
            blips.Add(blip);
            API.setBlipShortRange(blip.Blip, true);
        }
        dr.Close();

        API.consoleOutput("Blips: {0}", blips.Count);
    }
    #endregion

    #region ATMs
    private void CriarATM(Client player)
    {
        var str = string.Format(@"INSERT INTO atms (PosX, PosY, PosZ) VALUES ({0}, {1}, {2})",
           aFlt(player.position.X), aFlt(player.position.Y), aFlt(player.position.Z));
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;

        var atm = new ATM();
        atm.ID = id;
        atm.Posicao = player.position;
        atm.Text = API.createTextLabel("ATM", atm.Posicao, 30, (float)0.4);
        atms.Add(atm);
    }

    private void CarregarATMs()
    {
        var cmd = new MySqlCommand("SELECT * FROM atms", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var atm = new ATM();
            atm.ID = isInt(dr["ID"].ToString());
            atm.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            atm.Text = API.createTextLabel("ATM", atm.Posicao, 30, (float)0.4);
            atms.Add(atm);
        }
        dr.Close();

        API.consoleOutput("ATMs: {0}", atms.Count);
    }
    #endregion

    #region PEDs
    private void CriarPED(Client player, PedHash skin)
    {
        var ped = new adPed();
        ped.Posicao = player.position;
        ped.Dimensao = API.getEntityDimension(player);
        ped.Skin = skin;
        ped.Ped = API.createPed(ped.Skin, ped.Posicao, ped.Rotacao, ped.Dimensao);
        ped.Rotacao = 0;

        var str = string.Format(@"INSERT INTO peds (PosX, PosY, PosZ, Skin) VALUES ({0}, {1}, {2}, '{3}')",
           player.position.X.ToString().Replace(",", "."),
           player.position.Y.ToString().Replace(",", "."),
           player.position.Z.ToString().Replace(",", "."),
           ped.Ped.model
           );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        var id = (int)cmd.LastInsertedId;
        ped.ID = id;
        peds.Add(ped);
    }

    private void CarregarPEDs()
    {
        var cmd = new MySqlCommand("SELECT * FROM peds", bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var ped = new adPed();
            ped.ID = isInt(dr["ID"].ToString());
            ped.Posicao = new Vector3(isFloat(dr["PosX"].ToString()), isFloat(dr["PosY"].ToString()), isFloat(dr["PosZ"].ToString()));
            ped.Dimensao = isInt(dr["Dimensao"].ToString());
            ped.Rotacao = isInt(dr["Rotacao"].ToString());
            ped.Skin = (PedHash)isLong(dr["Skin"].ToString());
            ped.Ped = API.createPed(ped.Skin, ped.Posicao, ped.Rotacao, ped.Dimensao);
            peds.Add(ped);
        }
        dr.Close();

        API.consoleOutput("PEDs: {0}", peds.Count);
    }
    #endregion

    #region Funções
    private void VerificarCoisasCriadas(Client player)
    {
        if (API.hasEntitySyncedData(player, "pedlogin"))
        {
            var ped = (NetHandle)API.getEntitySyncedData(player, "pedlogin");
            API.deleteEntity(ped);
        }

        if (API.hasEntitySyncedData(player, "vehconce"))
        {
            var veh = (NetHandle)API.getEntitySyncedData(player, "vehconce");
            API.deleteEntity(veh);
        }

        if (API.hasEntitySyncedData(player, "vehconcetest"))
        {
            var veh = (NetHandle)API.getEntitySyncedData(player, "vehconcetest");
            API.deleteEntity(veh);
        }

        if (API.hasEntitySyncedData(player, "pedconce"))
        {
            var ped = (NetHandle)API.getEntitySyncedData(player, "pedconce");
            API.deleteEntity(ped);
        }

        if (API.hasEntitySyncedData(player, "pedautoescola"))
        {
            var ped = (NetHandle)API.getEntitySyncedData(player, "pedautoescola");
            API.deleteEntity(ped);
        }
    }

    private void ConfirmarVariacoes(Client player)
    {
        API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - VALOR_SKIN_VARIACAO);
        EnviarMensagemSucesso(player, string.Format("Variações compradas com sucesso. (${0})", VALOR_SKIN_VARIACAO));
    }

    private void ResetarVariacoes(Client player)
    {
        API.setPlayerDefaultClothes(player);
        if (API.hasEntitySyncedData(player, "variacoes"))
        {
            var variacoesAtuais = (List<object>)API.getEntitySyncedData(player, "variacoes");
            foreach (var str in variacoesAtuais)
            {
                API.setPlayerClothes(player, isInt(str.ToString().Split('|')[0]), isInt(str.ToString().Split('|')[1]), isInt(str.ToString().Split('|')[2]));
            }
        }
    }

    private char recuperarLetraRandom(int index)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return chars[index];
    }

    private string gerarPlacaVeiculo()
    {
        var random = new Random();
        var c1 = recuperarLetraRandom(random.Next(25));
        var c2 = recuperarLetraRandom(random.Next(25));
        var c3 = recuperarLetraRandom(random.Next(25));

        var placa = string.Format("{0}{1}{2}{3}",
            c1,
            c2,
            new Random().Next(0, 999).ToString().PadLeft(3, '0'),
            c3);
        return placa;
    }

    private void AceitarRecusarConvite(Client player, string opcao, string acao)
    {
        var strPlayer = string.Format("Você {0} o convite ", acao.Equals("aceitar") ? "aceitou" : "recusou");
        var strTarget = string.Format("{0} {1} o convite ", player.name, acao.Equals("aceitar") ? "aceitou" : "recusou");

        var target = recuperarClientPorID(API.getEntitySyncedData(player, "idconvidador"));

        switch (opcao)
        {
            case "faccao":
                if (API.getEntitySyncedData(player, "tipoconvite") != 1)
                {
                    EnviarMensagemErro(player, "Opção não confere com o tipo do convite.");
                    return;
                }

                strPlayer += "para entrar na facção.";
                strTarget += "para entrar na facção.";

                if (acao.Equals("aceitar"))
                {
                    API.setEntitySyncedData(player, "idfaccao", API.getEntitySyncedData(player, "idconvite"));
                    API.setEntitySyncedData(player, "idrank", 1);
                }

                break;
            case "prop":
                if (API.getEntitySyncedData(player, "tipoconvite") != 2)
                {
                    EnviarMensagemErro(player, "Opção não confere com o tipo do convite.");
                    return;
                }

                strPlayer += "para comprar a propriedade.";
                strTarget += "para comprar a propriedade.";

                if (acao.Equals("aceitar"))
                {
                    API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - API.getEntitySyncedData(player, "extraconvite"));
                    API.setEntitySyncedData(target, "dinheiro", API.getEntitySyncedData(target, "dinheiro") + API.getEntitySyncedData(player, "extraconvite"));

                    var str = string.Format("UPDATE propriedades SET IDPersonagemProprietario = {1} WHERE ID = {0}", API.getEntitySyncedData(player, "idconvite"), API.getEntitySyncedData(player, "id"));
                    var cmd = new MySqlCommand(str, bancodados);
                    cmd.ExecuteNonQuery();

                    var prop = (Propriedade)recuperarPropriedadePorID(API.getEntitySyncedData(player, "idconvite"));
                    var index = propriedades.IndexOf(prop);
                    propriedades[index].NomePersonagemProprietario = player.name;

                    RecarregarPropriedade(player, API.getEntitySyncedData(player, "idconvite"), "proprietario", Convert.ToString(API.getEntitySyncedData(player, "id")));
                }
                break;
            case "veiculo":
                if (API.getEntitySyncedData(player, "tipoconvite") != 3)
                {
                    EnviarMensagemErro(player, "Opção não confere com o tipo do convite.");
                    return;
                }

                strPlayer += "para comprar o veículo.";
                strTarget += "para comprar o veículo.";

                if (acao.Equals("aceitar"))
                {
                    API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - API.getEntitySyncedData(player, "extraconvite"));
                    API.setEntitySyncedData(target, "dinheiro", API.getEntitySyncedData(target, "dinheiro") + API.getEntitySyncedData(player, "extraconvite"));

                    var str = string.Format("UPDATE veiculos SET IDPersonagemProprietario={1} WHERE ID={0}", API.getEntitySyncedData(player, "idconvite"), API.getEntitySyncedData(player, "id"));
                    var cmd = new MySqlCommand(str, bancodados);
                    cmd.ExecuteNonQuery();

                    var veh = (Veiculo)recuperarVeiculoPorID(API.getEntitySyncedData(player, "idconvite"));
                    var index = veiculos.IndexOf(veh);
                    veiculos[index].IDPersonagemProprietario = API.getEntitySyncedData(player, "id");
                }
                break;
            default: EnviarMensagemErro(player, "Opção inválida."); return;
        }

        API.setEntitySyncedData(player, "tipoconvite", 0);
        API.setEntitySyncedData(player, "idconvite", 0);
        API.setEntitySyncedData(player, "idconvidador", 0);
        API.setEntitySyncedData(player, "extraconvite", 0);

        if (acao.Equals("aceitar"))
        {
            EnviarMensagemSucesso(player, strPlayer);
            if (target != null)
                EnviarMensagemSucesso(target, strTarget);
        }
        else
        {
            API.sendNotificationToPlayer(player, "~r~" + strPlayer);
            if (target != null)
                API.sendNotificationToPlayer(target, "~r~" + strTarget);
        }
    }

    private string encripWhirlpool(string str)
    {
        if (str.Trim().Equals(string.Empty)) return string.Empty;

        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

        Org.BouncyCastle.Crypto.Digests.WhirlpoolDigest w = new Org.BouncyCastle.Crypto.Digests.WhirlpoolDigest();

        byte[] data = encoding.GetBytes(str);
        w.Reset();
        w.BlockUpdate(data, 0, data.Length);

        byte[] o = new byte[w.GetDigestSize()];
        w.DoFinal(o, 0);
        return ByteToString(o).ToUpper();
    }

    private string ByteToString(byte[] buffer)
    {
        //for (int i = 0; i < buffer.Length; i++)
        //{
        //    sbinary += ((char) buffer[i]);
        //}
        string hex = BitConverter.ToString(buffer);
        return hex.Replace("-", "").ToLower();

        // return hex;
    }

    private string at(string txt)
    {
        return string.Format("'{0}'", txt.Replace("'", string.Empty));
    }

    private string aFlt(float valor)
    {
        return string.Format("'{0}'", valor.ToString().Replace(",", "."));
    }

    private void CriarLog(string nomeLog, string mensagem)
    {
        try
        {
            if (!Directory.Exists(DIR_LOGS)) Directory.CreateDirectory(DIR_LOGS);

            var strArquivoLog = string.Format("{0}\\{1}.txt", DIR_LOGS, nomeLog);
            var strLog = string.Format("{0} {1} - {2}\r\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), mensagem);

            File.AppendAllText(strArquivoLog, strLog);
        }
        catch (Exception ex)
        {
            API.consoleOutput("Erro ao criar log:" + ex.Message);
        }
    }

    private bool ChecarPlayerAnim(Client player)
    {
        var podeUsar = true;

        if (API.getEntitySyncedData(player, "ferido") || API.getEntitySyncedData(player, "morto") || API.getEntitySyncedData(player, "algemado"))
        {
            EnviarMensagemErro(player, "Você não pode usar/parar uma animação.");
            podeUsar = false;
        }

        return podeUsar;
    }

    private int isInt(string txt)
    {
        var ret = 0;
        int.TryParse(txt, out ret);
        return ret;
    }

    private float isFloat(string txt)
    {
        float ret = 0;
        float.TryParse(txt, out ret);
        return ret;
    }

    private long isLong(string txt)
    {
        long ret = 0;
        long.TryParse(txt, out ret);
        return ret;
    }

    private void AutenticarUsuario(Client player, string usuario, string senha, bool autenticacaoPorDialog = true)
    {
        var str = string.Format("SELECT * FROM usuarios WHERE NomeUsuario = {0}", at(usuario));
        var cmd = new MySqlCommand(str, bancodados);
        var dr = cmd.ExecuteReader();

        // Se o usuário existe
        if (dr.HasRows)
        {
            dr.Read();
            var idUsuario = isInt(dr["ID"].ToString());
            dr.Close();

            str = string.Format("SELECT * FROM usuarios WHERE NomeUsuario = {0} AND SenhaUsuario = {1}", at(usuario), at(encripWhirlpool(senha)));
            var cmd2 = new MySqlCommand(str, bancodados);
            var dr2 = cmd2.ExecuteReader();
            if (dr2.HasRows)
            {
                dr2.Read();
                // Acertou a senha
                API.setEntitySyncedData(player, "idusuario", idUsuario);
                API.setEntitySyncedData(player, "usuario", dr2["NomeUsuario"].ToString());
                API.setEntitySyncedData(player, "staff", isInt(dr2["Staff"].ToString()));
                API.setEntitySyncedData(player, "datahoraultimoacesso", dr2["DataHoraUltimoAcesso"].ToString());
                API.setEntitySyncedData(player, "quantidadesos", isInt(dr2["QuantidadeSOS"].ToString()));
                API.setEntitySyncedData(player, "tempoatrabalho", isInt(dr2["TempoATrabalho"].ToString()));
                dr2.Close();


                str = string.Format(@"SELECT ban.*, u.NomeUsuario NomeUsuarioStaff FROM banimentos ban
                    INNER JOIN usuarios u ON u.ID = ban.IDUsuarioStaff
                WHERE ban.IDUsuarioBanido = {0} OR ban.IPBanido = {1} OR ban.SocialClub = {2} ORDER BY ID DESC LIMIT 1", idUsuario, at(player.address), at(player.socialClubName));
                var cmd3 = new MySqlCommand(str, bancodados);
                var dr3 = cmd3.ExecuteReader();

                if (dr3.HasRows)
                {
                    dr3.Read();

                    var idBan = isInt(dr3["ID"].ToString());
                    var ipBanido = dr3["IPBanido"].ToString();
                    var socialClub = dr3["SocialClub"].ToString();
                    var expiracao = "N/A";
                    if (dr3["DataExpiracao"].ToString() != "")
                        expiracao = dr3["DataExpiracao"].ToString();

                    // Exibe dialog de banimento
                    var strban = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
                        idBan, dr3["DataHoraBanimento"].ToString(), expiracao, API.getEntitySyncedData(player, "usuario"),
                         dr3["SocialClub"].ToString(), dr3["MotivoBanimento"].ToString(), dr3["NomeUsuarioStaff"].ToString());
                    dr3.Close();

                    // Se for banimento temporário e tiver vencido desbane o player e chama a função novamente
                    if (expiracao != "N/A")
                    {
                        var exp = DateTime.Parse(expiracao);
                        if (DateTime.Now > exp)
                        {
                            str = string.Format(@"DELETE FROM banimentos WHERE ID = {0}", idBan);
                            cmd3 = new MySqlCommand(str, bancodados);
                            cmd3.ExecuteNonQuery();
                            AutenticarUsuario(player, usuario, senha, autenticacaoPorDialog);
                            return;
                        }

                    }

                    API.triggerClientEvent(player, "player_ban", strban);

                    // tentativa de burlar o ban em outro IP ou em outra soscial club
                    if (ipBanido != player.address || socialClub != player.socialClubName)
                    {
                        str = string.Format(@"SELECT * FROM tentativasburlarban WHERE IDBanimento = {0} AND (IP = {1} OR SocialClub = {2})  LIMIT 1",
                            idBan, at(player.address), at(player.socialClubName));
                        cmd3 = new MySqlCommand(str, bancodados);
                        var dr5 = cmd3.ExecuteReader();

                        if (!dr5.HasRows)
                        {
                            dr5.Close();
                            str = string.Format(@"INSERT INTO tentativasburlarban (IDBanimento, DataHoraTentativa, IP, SocialClub) 
                                VALUES ({0}, now(), {1}, {2})", idBan, at(player.address), at(player.socialClubName));
                            cmd = new MySqlCommand(str, bancodados);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            dr5.Close();
                        }
                    }
                    return;
                }
                else
                {
                    dr3.Close();
                }

                // Salva informações de ultimo acesso
                str = string.Format("UPDATE usuarios SET DataHoraUltimoAcesso = now(), IPUltimoAcesso = {0} WHERE ID = {1}", at(player.address), idUsuario);
                cmd2 = new MySqlCommand(str, bancodados);
                cmd2.ExecuteNonQuery();


                // boas vindas
                var msgboasvindas =
                    string.Format("Oi, {0}. Que bom te ver por aqui! Seu último acesso foi em {1}.",
                    API.getEntitySyncedData(player, "usuario"),
                    API.getEntitySyncedData(player, "datahoraultimoacesso").Equals(string.Empty) ? "N/A" : DateTime.Parse(API.getEntitySyncedData(player, "datahoraultimoacesso")));
                API.sendNotificationToPlayer(player, msgboasvindas);


                // Chama o menu de personagens
                str = string.Format("SELECT ID, NomePersonagem, Level, Skin, DataHoraUltimoAcesso FROM personagens WHERE IDUsuario={0} ORDER BY NomePersonagem", idUsuario);
                cmd2 = new MySqlCommand(str, bancodados);
                var dr4 = cmd2.ExecuteReader();
                var personagens = new List<string>();
                var descs = new List<string>();
                var descs_id = new List<int>();
                Ped ped = null;
                int PersonagemCustom = 0;
                int personagens_count = 0;
                while (dr4.Read())
                {
                    var ultimoacesso = "N/A";
                    if (dr4["DataHoraUltimoAcesso"].ToString() != "")
                        ultimoacesso = dr4["DataHoraUltimoAcesso"].ToString();

                    personagens.Add(dr4["NomePersonagem"].ToString().Replace("_", " "));
                    descs.Add(dr4["Skin"].ToString());
                    descs_id.Add(isInt(dr4["ID"].ToString()));

                    if (personagens_count == 0)
                    {
                        ped = API.createPed(API.pedNameToModel(dr4["Skin"].ToString()), new Vector3(402.9244, -996.288, -99.00025), 180, API.getEntitySyncedData(player, "dimensaorandom"));

                        if (dr4["Skin"].ToString() == "FreemodeMale01" || dr4["Skin"].ToString() == "FreemodeFemale01")
                            PersonagemCustom = isInt(dr4["ID"].ToString());
                    }
                    personagens_count++;
                }
                dr4.Close();

                if (PersonagemCustom > 0)
                {
                    var string_loadp = string.Format("SELECT * FROM personagem_custom WHERE PersID = {0} LIMIT 1", PersonagemCustom);
                    var cmd_loadp = new MySqlCommand(string_loadp, bancodados);
                    var lendoPers = cmd_loadp.ExecuteReader();

                    if (lendoPers.HasRows)
                    {
                        lendoPers.Read();
                        API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 3, isInt(lendoPers["Clothes_3"].ToString()), isInt(lendoPers["Clothes_3_T"].ToString()), 2);
                        API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 4, isInt(lendoPers["Clothes_4"].ToString()), isInt(lendoPers["Clothes_4_T"].ToString()), 2);
                        API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 6, isInt(lendoPers["Clothes_6"].ToString()), isInt(lendoPers["Clothes_6_T"].ToString()), 2);
                        API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 8, isInt(lendoPers["Clothes_8"].ToString()), isInt(lendoPers["Clothes_8_T"].ToString()), 2);
                        API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 11, isInt(lendoPers["Clothes_11"].ToString()), isInt(lendoPers["Clothes_11_T"].ToString()), 2);

                        API.sendNativeToPlayer(player, Hash.SET_PED_HEAD_BLEND_DATA, ped,
                                         isInt(lendoPers["GTAO_SHAPE_FIRST_ID"].ToString()), isInt(lendoPers["GTAO_SHAPE_SECOND_ID"].ToString()), 0,
                                         isInt(lendoPers["GTAO_SKIN_FIRST_ID"].ToString()), isInt(lendoPers["GTAO_SKIN_SECOND_ID"].ToString()), 0,
                                         isFloat(lendoPers["GTAO_SHAPE_MIX"].ToString()), isFloat(lendoPers["GTAO_SKIN_MIX"].ToString()), 0, false);

                        API.sendNativeToPlayer(player, Hash.SET_PED_COMPONENT_VARIATION, ped, 2, isInt(lendoPers["GTAO_HAIR_STYLE"].ToString()), 1);
                        API.sendNativeToPlayer(player, Hash._SET_PED_HAIR_COLOR, ped, isInt(lendoPers["GTAO_HAIR_COLOR"].ToString()), isInt(lendoPers["GTAO_HAIR_HIGHLIGHT_COLOR"].ToString()));

                        API.sendNativeToPlayer(player, Hash._SET_PED_EYE_COLOR, ped, isInt(lendoPers["GTAO_EYE_COLOR"].ToString()));

                        API.sendNativeToPlayer(player, Hash.SET_PED_HEAD_OVERLAY, ped, 2, isInt(lendoPers["GTAO_EYEBROWS"].ToString()), 1f);
                        API.sendNativeToPlayer(player, Hash._SET_PED_HEAD_OVERLAY_COLOR, ped, 2, 1, isInt(lendoPers["GTAO_EYEBROWS_COLOR"].ToString()), isInt(lendoPers["GTAO_EYEBROWS_COLOR2"].ToString()));

                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 0, isInt(lendoPers["GTAO_FACE_FEATURES_1"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 1, isInt(lendoPers["GTAO_FACE_FEATURES_2"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 2, isInt(lendoPers["GTAO_FACE_FEATURES_3"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 3, isInt(lendoPers["GTAO_FACE_FEATURES_4"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 4, isInt(lendoPers["GTAO_FACE_FEATURES_5"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 5, isInt(lendoPers["GTAO_FACE_FEATURES_6"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 6, isInt(lendoPers["GTAO_FACE_FEATURES_7"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 7, isInt(lendoPers["GTAO_FACE_FEATURES_8"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 8, isInt(lendoPers["GTAO_FACE_FEATURES_9"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 9, isInt(lendoPers["GTAO_FACE_FEATURES_10"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 10, isInt(lendoPers["GTAO_FACE_FEATURES_11"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 11, isInt(lendoPers["GTAO_FACE_FEATURES_12"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 12, isInt(lendoPers["GTAO_FACE_FEATURES_13"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 13, isInt(lendoPers["GTAO_FACE_FEATURES_14"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 14, isInt(lendoPers["GTAO_FACE_FEATURES_15"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 15, isInt(lendoPers["GTAO_FACE_FEATURES_16"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 16, isInt(lendoPers["GTAO_FACE_FEATURES_17"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 17, isInt(lendoPers["GTAO_FACE_FEATURES_18"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 18, isInt(lendoPers["GTAO_FACE_FEATURES_19"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 19, isInt(lendoPers["GTAO_FACE_FEATURES_20"].ToString()));
                        API.sendNativeToPlayer(player, Hash._SET_PED_FACE_FEATURE, ped, 20, isInt(lendoPers["GTAO_FACE_FEATURES_21"].ToString()));
                    }
                    lendoPers.Close();
                }

                if (personagens.Count.Equals(0))
                {
                    API.kickPlayer(player, "Você não possui nenhum personagem.");
                }
                else
                {
                    API.setEntitySyncedData(player, "selecioanndoPersonagem", true);
                    API.setEntitySyncedData(player, "pedlogin", ped);
                    API.setEntitySyncedData(player, "pedinfo", descs);
                    API.setEntitySyncedData(player, "pedinfo_id", descs_id);
                    API.setEntityPosition(player, new Vector3(402.9244, -1003.491, -99.00404));
                    API.triggerClientEvent(player, "criarcamera", new Vector3(402.9244, -1000.491, -99.00404), new Vector3(402.9244, -996.288, -99.00025));
                    API.triggerClientEvent(player, "listarpersonagens", personagens, descs);
                }
            }
            else
            {
                // Errou a senha
                dr2.Close();
                AdicionarTentativaLogin(player, idUsuario, autenticacaoPorDialog);
            }
        }
        else
        {
            dr.Close();
            AdicionarTentativaLogin(player, 0, autenticacaoPorDialog);
        }
    }

    private void AdicionarTentativaLogin(Client player, int idUsuario, bool autenticacaoPorDialog = true)
    {
        if (idUsuario != 0)
        {
            var str = string.Format("INSERT INTO tentativaslogins (DataHoraTentativa, IDUsuario, IP) VALUES (now(), {0}, {1})", idUsuario, at(player.address));
            var cmd = new MySqlCommand(str, bancodados);
            cmd.ExecuteNonQuery();
        }

        var erros = API.getEntitySyncedData(player, "errosautenticacao") + 1;
        API.sendNotificationToPlayer(player, string.Format("~r~Usuário ou senha inválida! ({0}/3)", erros));
        API.setEntitySyncedData(player, "errosautenticacao", erros);
        if (erros == 3)
        {
            API.setEntitySyncedData(player, "errosautenticacao", 0);
            player.kick("Você fez 3 tentativas de autenticação sem sucesso.");
        }
        else
        {
            if (autenticacaoPorDialog)
                API.triggerClientEvent(player, "player_login");
        }
    }

    private void ReviverPlayer(Client player)
    {
        API.setPlayerHealth(player, 50);
        API.setEntitySyncedData(player, "ferido", false);
        API.setEntitySyncedData(player, "morto", false);
        API.stopPlayerAnimation(player);
        API.freezePlayer(player, false);
        API.setEntityInvincible(player, false);
        API.setEntitySyncedData(player, "CAIDO_MORTO", 0);
        API.triggerClientEvent(player, "player_ragdoll_c");
    }

    #endregion

    #region Comandos
    [Command("adrp")]
    public void CMD_ADRP(Client player)
    {
        var lista = comandos.Where(c => API.getEntitySyncedData(player, "staff") >= c.Staff).ToList();
        API.triggerClientEvent(player, "player_help", API.toJson(lista));
    }

    [Command("id", "~y~USO: ~w~/id [id/ParteDoNome]")]
    public void CMD_ID(Client player, string idOrName)
    {
        var ids = findPlayer(player, idOrName, false);
    }

    [Command("login")]
    public void CMD_Login(Client player, string usuario, string senha)
    {
        if (API.hasEntitySyncedData(player, "logado"))
            return;

        if (API.hasEntitySyncedData(player, "selecioanndoPersonagem"))
            return;

        AutenticarUsuario(player, usuario, senha, false);
    }

    [Command("admins")]
    public void CMD_Admins(Client player)
    {
        var param1 = new List<string>();
        var param2 = new List<string>();
        foreach (var p in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(p, "logado"))
                if (API.getEntitySyncedData(p, "staff") > 0)
                {
                    param1.Add(string.Format("{0} ({1})", p.name.Replace("_", " "), API.getEntitySyncedData(p, "usuario")));
                    param2.Add(recuperarNomeStaff(API.getEntitySyncedData(p, "staff")));
                }
        }

        API.triggerClientEvent(player, "menusemresposta", "STAFF", "Membros da Staff", 6, param1, param2);
    }

    [Command("q")]
    public void CMD_Q(Client player)
    {
        player.kick("Quit");
    }

    [Command("aceitarmorte")]
    public void CMD_AceitarMorte(Client player)
    {
        if (!API.getEntitySyncedData(player, "morto") && API.getEntitySyncedData(player, "ferido"))
        {
            EnviarMensagemErro(player, "Você ainda não pode aceitar morte.");
            return;
        }
        else if (!API.getEntitySyncedData(player, "ferido"))
        {
            EnviarMensagemErro(player, "Você não está ferido.");
            return;
        }

        API.setEntityPosition(player, new Vector3(1816.001, 3679.644, 34.27679));
        ReviverPlayer(player);
    }

    [Command("limparmeuchat")]
    public void CMD_LimparMeuChat(Client player)
    {
        for (var i = 0; i < 15; i++)
            API.sendChatMessageToPlayer(player, " ");
    }

    [Command("pagar")]
    public void CMD_Pagar(Client player, string idOrName, int valor)
    {
        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }
        else if (player == target)
        {
            EnviarMensagemErro(player, "Você não pode pagar para si mesmo.");
            return;
        }
        else if (API.getEntitySyncedData(player, "dinheiro") < valor)
        {
            EnviarMensagemErro(player, "Você não possui o valor especificado.");
            return;
        }

        var players = API.getPlayersInRadiusOfPlayer(2, player).Where(p => p.name.Equals(target.name)).ToList();

        if (players.Count == 1)
        {

            API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - valor);
            API.setEntitySyncedData(target, "dinheiro", API.getEntitySyncedData(target, "dinheiro") + valor);
            EnviarMensagemSucesso(player, string.Format("Você deu ${0} para {1}.", valor, target.name.Replace("_", " ")));
            EnviarMensagemSucesso(target, string.Format("{0} deu ${1} para você.", player.name, valor));

            EnviarMensagemRadius(player, DISTANCIA_RP, string.Format("* {0} pagou para {1}.", player.name, target.name.Replace("_", " ")), COR_ROLEPLAY);

            CriarLog(LOG_PAGAMENTOS, string.Format("{0} ({1}) para {2} ({3}): {4}", player.name, player.address, target.name.Replace("_", " "), target.address, valor));
        }
        else
        {
            EnviarMensagemErro(player, string.Format("Você não está perto de {0}.", target.name.Replace("_", " ")));
        }
    }

    [Command("sos", GreedyArg = true)]
    public void CMD_SOS(Client player, string msg)
    {
        EnviarMensagemSOS(player, msg);
    }

    [Command("idveh")]
    public void CMD_IdVeh(Client player)
    {
        if (!player.isInVehicle)
        {
            EnviarMensagemErro(player, "Você não está em um veículo.");
            return;
        }

        var veh = recuperarVeiculo(player.vehicle);
        EnviarMensagemSucesso(player, string.Format("O ID do veículo é: {0}", veh.ID));
    }

    [Command("stats")]
    public void CMD_Stats(Client player)
    {
        var str = string.Format("{0}|{1}|{2}|{3}|{4}",
            player.name,
            player.health,
            player.armor,
            API.getEntitySyncedData(player, "dinheiro"),
            API.getEntitySyncedData(player, "banco"));

        API.triggerClientEvent(player, "player_stats", str);
    }

    [Command("mascara")]
    public void CMD_Mascara(Client player)
    {
        if (API.getEntitySyncedData(player, "idmascara") == 0)
        {
            EnviarMensagemErro(player, "Você não possui uma máscara.");
            return;
        }

        if (!player.nametag.Contains("Mascarado"))
        {
            API.setPlayerNametag(player, string.Format("Mascarado ({0})", API.getEntitySyncedData(player, "idmascara")));
            EnviarMensagemSucesso(player, "Você colocou sua máscara.");
        }
        else
        {
            API.setPlayerNametag(player, string.Format("{0} ({1})", player.name, recuperarIDPorClient(player)));
            API.sendNotificationToPlayer(player, "~r~Você retirou sua máscara.");
        }
    }

    [Command("tog")]
    public void CMD_Tog(Client player, string tipo)
    {
        switch (tipo)
        {
            case "chatooclocal":
                API.setEntitySyncedData(player, "togchatooclocal", !API.getEntitySyncedData(player, "togchatooclocal"));

                if (API.getEntitySyncedData(player, "togchatooclocal"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou o chat OOC local.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou o chat OOC local.");
                break;
            case "chatstaff":
                if (API.getEntitySyncedData(player, "staff") < 1)
                {
                    EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
                    return;
                }

                API.setEntitySyncedData(player, "togchatstaff", !API.getEntitySyncedData(player, "togchatstaff"));

                if (API.getEntitySyncedData(player, "togchatstaff"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou o chat da staff.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou o chat da staff.");
                break;
            case "staff":
                if (API.getEntitySyncedData(player, "staff") < 1)
                {
                    EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
                    return;
                }

                API.setEntitySyncedData(player, "togstaff", !API.getEntitySyncedData(player, "togstaff"));

                if (API.getEntitySyncedData(player, "togstaff"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou as mensagens da staff.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou as mensagens da staff.");
                break;
            case "chatfaccao":
                if (API.getEntitySyncedData(player, "idfaccao") == -1)
                {
                    EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
                    return;
                }

                API.setEntitySyncedData(player, "togchatfaccao", !API.getEntitySyncedData(player, "togchatfaccao"));

                if (API.getEntitySyncedData(player, "togchatfaccao"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou o chat da facção.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou o chat da facção.");
                break;
            case "faccao":
                if (API.getEntitySyncedData(player, "idfaccao") == -1)
                {
                    EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
                    return;
                }

                API.setEntitySyncedData(player, "togfaccao", !API.getEntitySyncedData(player, "togfaccao"));

                if (API.getEntitySyncedData(player, "togfaccao"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou as mensagens da facção.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou as mensagens da facção.");
                break;
            case "entradasaida":
                API.setEntitySyncedData(player, "togentradasaida", !API.getEntitySyncedData(player, "togentradasaida"));

                if (API.getEntitySyncedData(player, "togentradasaida"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou as mensagens de entrada e saída de jogadores.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou as mensagens de entrada e saída de jogadores.");
                break;
            case "pm":
                API.setEntitySyncedData(player, "togpm", !API.getEntitySyncedData(player, "togpm"));

                if (API.getEntitySyncedData(player, "togpm"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou as mensagens privadas.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou as mensagens privadas.");
                break;
            case "hud":
                API.setEntitySyncedData(player, "toghud", !API.getEntitySyncedData(player, "toghud"));

                if (API.getEntitySyncedData(player, "toghud"))
                    API.sendNotificationToPlayer(player, "~r~Você desabilitou a HUD.");
                else
                    EnviarMensagemSucesso(player, "Você habilitou a HUD.");
                break;
            default: EnviarMensagemErro(player, "Opção inválida."); break;
        }
    }

    [Command("aceitar")]
    public void CMD_Aceitar(Client player, string opcao)
    {
        AceitarRecusarConvite(player, opcao, "aceitar");
    }

    [Command("recusar")]
    public void CMD_Recusar(Client player, string opcao)
    {
        AceitarRecusarConvite(player, opcao, "recusar");
    }

    [Command("levelup")]
    public void CMD_LevelUP(Client player)
    {
        var xp = (API.getEntitySyncedData(player, "level") + 1) * 4;
        if (xp > API.getEntitySyncedData(player, "xp"))
        {
            EnviarMensagemErro(player, string.Format("Você não possui o XP necessário para subir de level. ({0}/{1})",
                API.getEntitySyncedData(player, "xp"),
                xp));
            return;
        }

        var novoLevel = API.getEntitySyncedData(player, "level") + 1;
        API.setEntitySyncedData(player, "level", novoLevel);
        API.setEntitySyncedData(player, "xp", 0);

        EnviarMensagemSucesso(player, string.Format("Você subiu para o level {0}.", novoLevel));
    }

    [Command("atm")]
    public void CMD_ATM(Client player)
    {
        foreach (var atm in atms)
        {
            var play = API.getPlayersInRadiusOfPosition(3, atm.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                var str = string.Format("{0}|{1}",
                    player.name,
                    API.getEntitySyncedData(player, "banco"));

                API.triggerClientEvent(player, "atm", str);
                return;
            }
        }

        EnviarMensagemErro(player, "Você não está próximo de uma ATM.");
    }

    [Command("uvehid")]
    public void CMD_UVehID(Client player)
    {
        if (API.hasEntitySyncedData(player, "uvehid"))
            EnviarMensagemSucesso(player, "Último veículo: " + API.getEntitySyncedData(player, "uvehid"));
    }

    [Command("cinto")]
    public void CMD_Cinto(Client player)
    {
        if (!player.isInVehicle)
        {
            EnviarMensagemErro(player, "Você não está em um veículo.");
            return;
        }

        API.setPlayerSeatbelt(player, !API.getPlayerSeatbelt(player));

        if (API.getPlayerSeatbelt(player))
            EnviarMensagemSucesso(player, "Você colocou o cinto de segurança.");
        else
            API.sendNotificationToPlayer(player, "~r~Você retirou o cinto de segurança.");
    }

    [Command("comprarskin")]
    public void CMD_ComprarSkin(Client player, string skin)
    {
        var sk = API.pedNameToModel(skin);
        if (sk == 0)
        {
            EnviarMensagemErro(player, "A skin informada não existe.");
            return;
        }

        var s = SKINS_PROIBIDAS_COMPRA.Where(ski => ski.ToLower().Equals(skin.ToLower())).ToList();
        if (s.Count > 0)
        {
            EnviarMensagemErro(player, "Você não pode comprar essa skin.");
            return;
        }

        foreach (var p in pontos.Where(t => t.TipoPonto.Equals(Ponto.Tipo.Skin)).ToList())
        {
            var ply = API.getPlayersInRadiusOfPosition(3, p.Posicao).Where(pl => pl.name.Equals(player.name)).ToList();
            if (ply.Count > 0)
            {
                if (API.getEntitySyncedData(player, "dinheiro") < VALOR_SKIN)
                {
                    EnviarMensagemErro(player, string.Format("Você não possui dinheiro suficiente. (${0})", VALOR_SKIN));
                    return;
                }

                API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - VALOR_SKIN);

                var weaponData = new Dictionary<WeaponHash, CWeaponData>();
                foreach (WeaponHash wepHash in API.getPlayerWeapons(player))
                    weaponData.Add(wepHash,
                        new CWeaponData
                        {
                            Ammo = API.getPlayerWeaponAmmo(player, wepHash),
                            Tint = API.getPlayerWeaponTint(player, wepHash),
                            Components = JsonConvert.SerializeObject(API.getPlayerWeaponComponents(player, wepHash))
                        });

                API.setPlayerSkin(player, sk);

                foreach (var weapon in weaponData)
                {
                    API.givePlayerWeapon(player, weapon.Key, weapon.Value.Ammo, false, true);
                    API.setPlayerWeaponTint(player, weapon.Key, weapon.Value.Tint);

                    var weaponMods = JsonConvert.DeserializeObject<List<WeaponComponent>>(weapon.Value.Components);
                    foreach (WeaponComponent compID in weaponMods) API.givePlayerWeaponComponent(player, weapon.Key, compID);
                }

                EnviarMensagemSucesso(player, "Skin comprada com sucesso.");
                return;
            }
        }

        EnviarMensagemErro(player, "Você não está em um ponto de venda de skin.");
    }

    [Command("comprarvariacao")]
    public void CMD_ComprarVariacao(Client player)
    {
        foreach (var p in pontos.Where(t => t.TipoPonto.Equals(Ponto.Tipo.Skin)).ToList())
        {
            var ply = API.getPlayersInRadiusOfPosition(3, p.Posicao).Where(pl => pl.name.Equals(player.name)).ToList();
            if (ply.Count > 0)
            {
                if (API.getEntitySyncedData(player, "dinheiro") < VALOR_SKIN_VARIACAO)
                {
                    EnviarMensagemErro(player, string.Format("Você não possui dinheiro suficiente. (${0})", VALOR_SKIN_VARIACAO));
                    return;
                }

                var variacoesAtuais = new List<object>();
                for (var i = 0; i <= 11; i++)
                {
                    variacoesAtuais.Add(string.Format("{0}|{1}|{2}",
                       i, API.getPlayerClothesDrawable(player, i), API.getPlayerClothesTexture(player, i)));
                }
                API.setEntitySyncedData(player, "variacoes", variacoesAtuais);

                API.triggerClientEvent(player, "comprarvariacao");
                return;
            }
        }

        EnviarMensagemErro(player, "Você não está em um ponto de venda de skin.");
    }

    [Command("dropar")]
    public void CMD_dropar(Client player, string item, int quantidade = 0)
    {
        if (item == "")
        {
            EnviarMensagemErro(player, "USE: /dropar [item]\nItens: Arma, Dinheiro");
            return;
        }
        switch (item)
        {
            case "dinheiro":
                if (quantidade <= 0) { EnviarMensagemErro(player, "A quantidade precisa ser maior que $0."); return; }
                if (API.getEntitySyncedData(player, "dinheiro") < quantidade) { EnviarMensagemErro(player, "Você não tem $" + quantidade + " para dropar."); return; }

                API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - quantidade);
                DroparItem(player, 1, "", quantidade);
                break;
            case "arma":
                switch (API.getPlayerCurrentWeapon(player).ToString())
                {
                    case "Unarmed":
                        EnviarMensagemErro(player, "Você não tem uma arma em mãos.");
                        break;
                    default:
                        DroparItem(player, 2, API.getPlayerCurrentWeapon(player).ToString(), API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player)));
                        break;
                }
                break;

        }
        return;
    }
    [Command("pegar")]
    public void CMD_pegar(Client player)
    {
        foreach (var it in itensDropados)
        {
            var itens = API.getPlayersInRadiusOfPosition(2, it.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
            {
                API.sendChatMessageToPlayer(player, string.Format("Item {0}: {1} | {2}", it.ID, it.TipoItem, it.Quantidade));

                API.setEntityPositionFrozen(it.Objeto, false);

                API.deleteEntity(it.Objeto);
                var str = string.Format("DELETE FROM itens_dropados WHERE ID = {0}", it.ID);
                var cmd = new MySqlCommand(str, bancodados);
                cmd.ExecuteNonQuery();

                if (it.TipoItem == ItensDropados.Tipo.Dinheiro)
                {
                    API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") + it.Quantidade);
                    EnviarMensagemSucesso(player, string.Format("Você pegou ${0} do chão.", it.Quantidade));
                    itensDropados.Remove(it);
                }
                else if (it.TipoItem == ItensDropados.Tipo.Arma)
                {
                    WeaponHash[] playerWeapons = API.getPlayerWeapons(player);
                    int JaTemAArma = 0;
                    foreach (WeaponHash weapon in playerWeapons)
                    {
                        if (weapon == API.weaponNameToModel(it.Modelo))
                        {
                            JaTemAArma = 1;
                        }
                    }

                    if (JaTemAArma == 0)
                    {
                        API.givePlayerWeapon(player, API.weaponNameToModel(it.Modelo), it.Quantidade, true, true);
                        EnviarMensagemSucesso(player, string.Format("Você pegou uma {0} com {1} balas do chão.", it.Modelo, it.Quantidade));


                        int return_w = RetornarHashArmaByName(it.Modelo);
                        if (return_w != 99)
                        {
                            if (return_w != 0)
                            {
                                API.sendNativeToPlayer(player, Hash.SET_CURRENT_PED_WEAPON, player, return_w, 1);
                            }
                        }
                        else
                            EnviarMensagemErro(player, "deu ruim #0025");

                        itensDropados.Remove(it);
                    }
                    else
                    {
                        EnviarMensagemErro(player, string.Format("Você já tem uma {1} em mãos, guarde-a no inventário antes.", it.Modelo, it.Quantidade));
                    }
                }

                return;
            }
        }
    }
    #endregion

    #region Comandos Facções
    [Command("membros")]
    public void CMD_Membros(Client player)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        var param1 = new List<string>();
        var param2 = new List<string>();
        foreach (var p in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(p, "logado") && faccao.ID == API.getEntitySyncedData(p, "idfaccao"))
            {
                param1.Add(string.Format("{0} ({1})", p.name.Replace("_", " "), API.getEntitySyncedData(p, "usuario")));
                param2.Add(recuperarFaccaoRankPorID(faccao.ID, API.getEntitySyncedData(p, "idrank")).NomeRank);
            }
        }

        API.triggerClientEvent(player, "menusemresposta", faccao.AbreviaturaFaccao, "Membros da Facção", 6, param1, param2);
    }

    [Command("blockf")]
    public void CMD_Blockf(Client player)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        var index = faccoes.IndexOf(faccao);

        if (API.getEntitySyncedData(player, "idrank") < faccoes[index].IDRankGestor)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        faccoes[index].ChatBloqueado = !faccoes[index].ChatBloqueado;

        var str = string.Empty;
        if (faccoes[index].ChatBloqueado)
            str = string.Format("~r~{0} bloqueou o chat da facção.", API.getEntitySyncedData(player, "usuario"));
        else
            str = string.Format("~g~{0} ativou o chat da facção.", API.getEntitySyncedData(player, "usuario"));

        EnviarMensagemOOCFaccao(API.getEntitySyncedData(player, "idfaccao"), str);
    }

    [Command("rank")]
    public void CMD_Rank(Client player, string idOrName, int rank)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        var index = faccoes.IndexOf(faccao);

        if (API.getEntitySyncedData(player, "idrank") < faccoes[index].IDRankGestor)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (rank < 1 || rank > MAX_FACCOES_RANKS)
        {
            EnviarMensagemErro(player, string.Format("O rank precisa ser entre 1 e {0}.", MAX_FACCOES_RANKS));
            return;
        }

        if (API.getEntitySyncedData(player, "idfaccao") != API.getEntitySyncedData(player, "idfaccao"))
        {
            EnviarMensagemErro(player, "O jogador não está em sua facção.");
            return;
        }

        API.setEntitySyncedData(target, "idrank", rank);

        EnviarMensagemSucesso(player, string.Format("Você alterou o rank de {0} para {1}.", target.name.Replace("_", " "),
            recuperarFaccaoRankPorID(API.getEntitySyncedData(player, "idfaccao"), rank).NomeRank));
        EnviarMensagemSucesso(target, string.Format("{0} alterou seu rank para {1}.", API.getEntitySyncedData(player, "usuario"),
            recuperarFaccaoRankPorID(API.getEntitySyncedData(player, "idfaccao"), rank).NomeRank));
    }

    [Command("expulsar")]
    public void CMD_Expulsar(Client player, string idOrName)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        var index = faccoes.IndexOf(faccao);

        if (API.getEntitySyncedData(player, "idrank") < faccoes[index].IDRankGestor)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getEntitySyncedData(player, "idfaccao") != API.getEntitySyncedData(player, "idfaccao"))
        {
            EnviarMensagemErro(player, "O jogador não está em sua facção.");
            return;
        }

        API.setEntitySyncedData(target, "idfaccao", 0);
        API.setEntitySyncedData(target, "idrank", 0);

        EnviarMensagemSucesso(player, string.Format("Você expulsou {0} da facção.", target.name.Replace("_", " ")));
        EnviarMensagemSucesso(target, string.Format("{0} expulsou você da facção.", API.getEntitySyncedData(player, "usuario")));
    }

    [Command("convidar")]
    public void CMD_Convidar(Client player, string idOrName)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        var index = faccoes.IndexOf(faccao);

        if (API.getEntitySyncedData(player, "idrank") < faccoes[index].IDRankGestor)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getEntitySyncedData(target, "idfaccao") != 0)
        {
            EnviarMensagemErro(player, "O jogador já está em uma facção.");
            return;
        }

        API.setEntitySyncedData(target, "tipoconvite", 1);
        API.setEntitySyncedData(target, "idconvite", API.getEntitySyncedData(player, "idfaccao"));
        API.setEntitySyncedData(target, "idconvidador", recuperarIDPorClient(player));

        EnviarMensagemSucesso(player, string.Format("Você convidou {0} para a facção.", target.name.Replace("_", " ")));
        EnviarMensagemSucesso(target, string.Format("{0} convidou você para a facção. (/aceitar faccao ou /recusar faccao)", API.getEntitySyncedData(player, "usuario")));
    }

    [Command("armario")]
    public void CMD_Armario(Client player)
    {
        foreach (var arm in armarios)
        {
            var play = API.getPlayersInRadiusOfPosition(3, arm.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0
                && arm.IDFaccao.Equals(API.getEntitySyncedData(player, "idfaccao"))
                && API.getEntitySyncedData(player, "idrank") >= arm.IDRank)
            {
                var param1 = new List<string>();
                var param2 = new List<string>();
                foreach (var it in arm.Itens)
                {
                    if (API.getEntitySyncedData(player, "idrank") >= it.IDRank)
                    {
                        param1.Add(it.Arma);
                        param2.Add(string.Format("Munição: {0} | Estoque: {1}", it.Municao, it.Estoque));
                    }
                }

                API.triggerClientEvent(player, "menucomresposta", "ARMÁRIO " + arm.ID, "Listagem de Itens", 6, param1, param2);
                return;
            }
        }

        EnviarMensagemErro(player, "Você não está próximo de nenhum armário.");
    }

    [Command("sairfaccao")]
    public void CMD_SairFaccao(Client player)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        API.setEntitySyncedData(player, "idfaccao", 0);
        API.setEntitySyncedData(player, "idrank", 0);

        EnviarMensagemSucesso(player, "Você saiu da facção.");
    }
    #endregion

    #region Comandos Policiais
    [Command("r", GreedyArg = true)]
    public void CMD_R(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "canalradio") == 0)
        {
            EnviarMensagemErro(player, "Canal de rádio principal não configurado.");
            return;
        }

        EnviarMensagemRadio(player, 1, API.getEntitySyncedData(player, "canalradio"), msg);
    }

    [Command("r2", GreedyArg = true)]
    public void CMD_R2(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "canalradio2") == 0)
        {
            EnviarMensagemErro(player, "Canal de rádio secundário não configurado.");
            return;
        }

        EnviarMensagemRadio(player, 2, API.getEntitySyncedData(player, "canalradio2"), msg);
    }

    [Command("r3", GreedyArg = true)]
    public void CMD_R3(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "canalradio3") == 0)
        {
            EnviarMensagemErro(player, "Canal de rádio terciário não configurado.");
            return;
        }

        EnviarMensagemRadio(player, 3, API.getEntitySyncedData(player, "canalradio3"), msg);
    }

    [Command("canal")]
    public void CMD_Canal(Client player, int slot, int canal)
    {
        if (slot < 1 || slot > 3)
        {
            EnviarMensagemErro(player, "O slot precisa ser entre 1 e 3.");
            return;
        }

        if (canal < 0)
        {
            EnviarMensagemErro(player, "O canal precisa ser maior que 0.");
            return;
        }

        if (slot == 1)
            API.setEntitySyncedData(player, "canalradio", canal);
        else
            API.setEntitySyncedData(player, "canalradio" + slot, canal);

        EnviarMensagemSucesso(player, string.Format("Você alterou o canal de rádio do slot {0} para {1}.", slot, canal));
    }

    [Command("m", GreedyArg = true)]
    public void CMD_M(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "idfaccao") == 0)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var tipoFac = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao")).TipoFaccao;
        if (tipoFac != Faccao.Tipo.Policial)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        EnviarMensagemRadius(player, 40, string.Format("[MEGAFONE] {0} diz: {1}", player.name, msg), COR_MEGAFONE);
    }

    [Command("hq", GreedyArg = true)]
    public void CMD_HQ(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "idfaccao") == 0)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        var tipoFac = faccao.TipoFaccao;
        if (!(tipoFac == Faccao.Tipo.Policial || tipoFac == Faccao.Tipo.Governamental || tipoFac == Faccao.Tipo.Bombeiros)
            || API.getEntitySyncedData(player, "idrank") + 1 < faccao.IDRankGestor)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        EnviarMensagemFaccao(API.getEntitySyncedData(player, "idfaccao"), string.Format("[HQ] {0}", msg));
    }

    [Command("gov", GreedyArg = true)]
    public void CMD_GOV(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "idfaccao") == 0)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        var tipoFac = faccao.TipoFaccao;
        if (!(tipoFac == Faccao.Tipo.Policial || tipoFac == Faccao.Tipo.Governamental || tipoFac == Faccao.Tipo.Bombeiros)
            || API.getEntitySyncedData(player, "idrank") + 1 < faccao.IDRankGestor)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        foreach (var target in API.getAllPlayers())
        {
            if (API.hasEntitySyncedData(target, "logado"))
            {
                API.sendChatMessageToPlayer(target, string.Format("~#{0}~", faccao.CorFaccao), faccao.NomeFaccao);
                API.sendChatMessageToPlayer(target, msg);
            }
        }
    }

    [Command("trabalho")]
    public void CMD_Trabalho(Client player)
    {
        var faccao = (Faccao)recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }
        if (!faccao.TipoFaccao.Equals(Faccao.Tipo.Policial))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.setEntitySyncedData(player, "trabalho", !API.getEntitySyncedData(player, "trabalho"));

        if (API.getEntitySyncedData(player, "trabalho"))
        {
            var cor = System.Drawing.ColorTranslator.FromHtml("#" + faccao.CorFaccao);
            API.setPlayerNametagColor(player, cor.R, cor.G, cor.B);

            EnviarMensagemSucesso(player, "Você entrou em trabalho.");
        }
        else
        {
            API.setPlayerNametagColor(player, 255, 255, 255);
            EnviarMensagemSucesso(player, "Você está fora do serviço.");
        }
    }

    [Command("uniforme")]
    public void CMD_Uniforme(Client player)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (!(faccao.TipoFaccao == Faccao.Tipo.Policial || faccao.TipoFaccao == Faccao.Tipo.Bombeiros))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        foreach (var ponto in pontos.Where(p => p.TipoPonto == Ponto.Tipo.Uniforme))
        {
            var players = API.getPlayersInRadiusOfPosition(3, ponto.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (players.Count > 0)
            {
                var param1 = new List<string>();
                var param2 = new List<string>();

                if (faccao.TipoFaccao == Faccao.Tipo.Policial)
                {
                    param1.Add("Cop01SMY");
                    param1.Add("Cop01SFY");
                    param1.Add("HWayCop01SMY");
                    param1.Add("SWAT01SMY");
                    param1.Add("Civil");
                    param2.Add(string.Empty);
                    param2.Add(string.Empty);
                    param2.Add(string.Empty);
                    param2.Add(string.Empty);
                    param2.Add(string.Empty);
                }
                else if (faccao.TipoFaccao == Faccao.Tipo.Bombeiros)
                {
                    param1.Add("Fireman01SMY");
                    param1.Add("Paramedic01SMM");
                    param1.Add("Civil");
                    param2.Add(string.Empty);
                    param2.Add(string.Empty);
                    param2.Add(string.Empty);
                }

                API.triggerClientEvent(player, "menucomresposta", "UNIFORMES", "Listagem de Uniformes", 6, param1, param2);
                return;
            }
        }

        EnviarMensagemErro(player, "Você não está próximo a um ponto de uniforme.");
    }

    [Command("portao")]
    public void CMD_Portao(Client player)
    {
        if (API.getEntitySyncedData(player, "idfaccao") == 0)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }
        var tipoFac = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao")).TipoFaccao;
        if (tipoFac != Faccao.Tipo.Policial)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (PortaoLSPD_Fundos_Status == 0)
        {
            EnviarMensagemSucesso(player, "Portão fechado");
            API.exported.doormanager.transitionDoor(PortaoLSPD_Fundos, 0, 3000);
            PortaoLSPD_Fundos_Status = 1;
        }
        else
        {
            EnviarMensagemSucesso(player, "Portão aberto");
            API.exported.doormanager.transitionDoor(PortaoLSPD_Fundos, 1, 3000);
            PortaoLSPD_Fundos_Status = 0;
        }
    }

    [Command("algemar")]
    public void CMD_Algemar(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "idfaccao") == 0)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var tipoFac = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao")).TipoFaccao;
        if (tipoFac != Faccao.Tipo.Policial)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        var players = API.getPlayersInRadiusOfPlayer(3, player).Where(p => p.name.Equals(target.name)).ToList();
        if (players.Count == 0)
        {
            EnviarMensagemErro(player, "Você não está próximo do player.");
            return;
        }

        API.setEntitySyncedData(target, "algemado", !API.getEntitySyncedData(target, "algemado"));
        API.stopPlayerAnimation(target);

        if (API.getEntitySyncedData(target, "algemado"))
        {
            //var algema = API.createObject(-1281059971, new Vector3(), new Vector3(), API.getEntityDimension(target));
            //API.attachEntityToEntity(algema, target, "SKEL_R_Hand", new Vector3(), new Vector3());
            //API.setEntitySyncedData(target, "objalgema", algema);
            EnviarMensagemSucesso(player, string.Format("Você algemou {0}.", recuperarNomeIC(target)));
            EnviarMensagemSucesso(target, string.Format("{0} algemou você.", recuperarNomeIC(player)));
            API.playPlayerAnimation(target, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle");
        }
        else
        {
            API.sendNativeToPlayer(player, Hash.SET_ENABLE_HANDCUFFS, target, false);
            //API.deleteEntity(API.getEntitySyncedData(target, "objalgema"));
            EnviarMensagemSucesso(player, string.Format("Você desalgemou {0}.", recuperarNomeIC(target)));
            EnviarMensagemSucesso(target, string.Format("{0} desalgemou você.", recuperarNomeIC(player)));
        }

        /*if (API.getEntitySyncedData(target, "algemado"))
        {
            var algema = API.createObject(, new Vector3(), new Vector3(), API.getEntityDimension(player));
            API.attachEntityToEntity(algema, player, "SKEL_R_Hand", new Vector3(), new Vector3());
            API.setEntitySyncedData(player, "objalgema", algema);
            API.playPlayerAnimation(target, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle");
            EnviarMensagemSucesso(player, string.Format("Você algemou {0}.", recuperarNomeIC(target)));
            EnviarMensagemSucesso(target, string.Format("{0} algemou você.", recuperarNomeIC(player)));
        }
        else
        {
            EnviarMensagemSucesso(player, string.Format("Você desalgemou {0}.", recuperarNomeIC(target)));
            EnviarMensagemSucesso(target, string.Format("{0} desalgemou você.", recuperarNomeIC(player)));
        }*/
    }

    [Command("prender", "~y~USO: ~w~/prender [idOrName] [cela (1-3)] [tempo (Minutos)]")]
    public void CMD_Prender(Client player, string idOrName, int cela, int tempo)
    {
        var tipoFac = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao")).TipoFaccao;
        if (tipoFac != Faccao.Tipo.Policial)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var play = API.getPlayersInRadiusOfPosition(2, new Vector3(464.0645, -997.7375, 24.91487)).Where(p => p.name.Equals(player.name)).ToList();
        if (play.Count > 0)
        {
            var target = findPlayer(player, idOrName);
            if (target == null)
            {
                EnviarMensagemErro(player, "Jogador não encontrado.");
                return;
            }

            var players = API.getPlayersInRadiusOfPlayer(3, player).Where(p => p.name.Equals(target.name)).ToList();
            if (players.Count == 0)
            {
                EnviarMensagemErro(player, "Você não está próximo do player.");
                return;
            }

            switch (cela)
            {
                case 1:
                    API.setEntityPosition(target, new Vector3(459.6541, -994.0992, 24.91488));
                    break;
                case 2:
                    API.setEntityPosition(target, new Vector3(459.4538, -997.7994, 24.91487));
                    break;
                case 3:
                    API.setEntityPosition(target, new Vector3(459.3237, -1001.342, 24.91488));
                    break;
            }

            EnviarMensagemSucesso(player, "Você prendeu " + target.name);
            EnviarMensagemSucesso(player, player.name + " te prendeu por " + tempo + " minutos.");
        }
        else
        {
            EnviarMensagemErro(player, "Você não está em frente as celas do departamento.");
            return;
        }
    }

    [Command("visao")]
    public void CMD_Visao(Client player, string opcao)
    {
        if (API.getEntitySyncedData(player, "idfaccao") == 0)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var tipoFac = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao")).TipoFaccao;
        if (tipoFac != Faccao.Tipo.Policial)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if ((opcao.Equals("calor") || opcao.Equals("noturna")))
        {
            if (!((PedHash)player.model).ToString().ToLower().Equals("swat01smy"))
            {
                if (!player.isInVehicle)
                {
                    EnviarMensagemErro(player, "Você não está em um helicóptero policial ou com o uniforme da SWAT.");
                    return;
                }

                if (!((VehicleHash)player.vehicle.model).ToString().ToLower().Equals("polmav"))
                {
                    EnviarMensagemErro(player, "Você não está em um helicóptero policial ou com o uniforme da SWAT.");
                    return;
                }
            }
        }

        switch (opcao)
        {
            case "des":
                API.triggerClientEvent(player, "desligarvisao", "0x18F621F7A5B1F85D");
                API.triggerClientEvent(player, "desligarvisao", "0x7E08924259E08CE0");
                EnviarMensagemSubtitulo(player, "Visão de calor/norturna ~r~desligada");
                break;
            case "calor":
                API.triggerClientEvent(player, "ligarvisao", "0x7E08924259E08CE0");
                EnviarMensagemSubtitulo(player, "Visão de calor ~g~ligada");
                break;
            case "noturna":
                API.triggerClientEvent(player, "ligarvisao", "0x18F621F7A5B1F85D");
                EnviarMensagemSubtitulo(player, "Visão norturna ~g~ligada");
                break;
            default: EnviarMensagemErro(player, MSG_OPCAO_INVALIDA); break;
        }
    }
    #endregion

    #region Comandos Chat OOC
    [Command("pm", GreedyArg = true)]
    public void CMD_PM(Client player, string idOrName, string msg)
    {
        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }
        if (player == target)
        {
            EnviarMensagemErro(player, "Você não pode enviar uma PM para si mesmo.");
            return;
        }
        if (API.getEntitySyncedData(player, "togpm"))
        {
            EnviarMensagemErro(player, "Você desabilitou as mensagens privadas.");
            return;
        }
        if (API.getEntitySyncedData(target, "togpm"))
        {
            EnviarMensagemErro(player, "O player desabilitou as mensagens privadas.");
            return;
        }

        API.sendChatMessageToPlayer(player, COR_PM_ENVIADA, string.Format("(( PM para {0} [{1}]: {2} ))", target.name.Replace("_", " "), recuperarIDPorClient(target), msg));
        API.sendChatMessageToPlayer(target, COR_PM_RECEBIDA, string.Format("(( PM de {0} [{1}]: {2} ))", player.name, recuperarIDPorClient(player), msg));
    }

    [Command("o", GreedyArg = true)]
    public void CMD_O(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.sendChatMessageToAll(COR_CHAT_OOC_GLOBAL, string.Format("(( {0}: {1} ))", API.getEntitySyncedData(player, "usuario"), msg));
    }

    [Command("b", GreedyArg = true)]
    public void CMD_B(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "togchatooclocal"))
        {
            EnviarMensagemErro(player, "Você desabilitou o chat OOC local.");
            return;
        }

        EnviarMensagemChatOOCLocal(player, msg);
    }

    [Command("f", GreedyArg = true)]
    public void CMD_F(Client player, string msg)
    {
        var faccao = recuperarFaccaoPorID(API.getEntitySyncedData(player, "idfaccao"));
        if (faccao == null)
        {
            EnviarMensagemErro(player, MSG_SEM_FACCAO);
            return;
        }

        if (API.getEntitySyncedData(player, "togchatfaccao"))
        {
            EnviarMensagemErro(player, "Você desabilitou o chat da facção.");
            return;
        }

        if (faccao.ChatBloqueado)
        {
            EnviarMensagemErro(player, "O chat da facção está bloqueado.");
            return;
        }

        EnviarMensagemChatOOCFaccao(player, msg);
    }
    #endregion

    #region Comandos Chat IC
    [Command("g", GreedyArg = true)]
    public void CMD_G(Client player, string msg)
    {
        EnviarMensagemRadius(player, DISTANCIA_RP * 2, recuperarNomeIC(player) + " grita: " + msg);
    }

    [Command("baixo", GreedyArg = true)]
    public void CMD_Baixo(Client player, string msg)
    {
        EnviarMensagemRadius(player, 3, recuperarNomeIC(player) + " diz baixo: " + msg);
    }

    [Command("cw", GreedyArg = true)]
    public void CMD_CW(Client player, string msg)
    {
        if (player.vehicle == null)
        {
            EnviarMensagemErro(player, "Você não está em um veículo.");
            return;
        }

        var status = "Passageiro ";

        if (player.vehicleSeat == -1)
            status = "Motorista ";

        EnviarMensagemVeiculo(player, status + recuperarNomeIC(player) + " diz: " + msg);
    }

    [Command("s", GreedyArg = true)]
    public void CMD_S(Client player, string idOrName, string msg)
    {
        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }
        else if (player == target)
        {
            EnviarMensagemErro(player, "Você não pode sussurrar para si mesmo.");
            return;
        }

        var players = API.getPlayersInRadiusOfPlayer(2, player).Where(p => p.name.Equals(target.name)).ToList();

        if (players.Count == 1)
        {
            API.sendChatMessageToPlayer(player, COR_PM_ENVIADA, string.Format("{0} sussurra: {1}", recuperarNomeIC(player), msg));
            API.sendChatMessageToPlayer(target, COR_PM_RECEBIDA, string.Format("{0} sussurra: {1}", recuperarNomeIC(player), msg));

            EnviarMensagemRadius(player, DISTANCIA_RP, string.Format("* {0} sussurra para {1}.", recuperarNomeIC(player), recuperarNomeIC(target)), COR_ROLEPLAY);
        }
        else
        {
            EnviarMensagemErro(player, string.Format("Você não está perto de {0}.", recuperarNomeIC(target)));
        }
    }

    [Command("ds", GreedyArg = true)]
    public void CMD_DS(Client player, string msg)
    {
        foreach (var prop in propriedades)
        {
            if (prop.EntradaFrente.X == 0 || prop.SaidaFrente.X == 0 || prop.Interior == 0)
                continue;

            var play = API.getPlayersInRadiusOfPosition(2, prop.EntradaFrente).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                msg = string.Format("{0} grita: {1}", recuperarNomeIC(player), msg);
                foreach (var target in API.getPlayersInRadiusOfPosition(20, prop.SaidaFrente).Where(p => prop.ID.Equals(API.getEntitySyncedData(p, "idpropriedade"))))
                {
                    API.sendChatMessageToPlayer(target, "~#B5B5B5~", msg);
                }
                API.sendChatMessageToPlayer(player, "~#FFFFFF~", msg);
                return;
            }
        }
        EnviarMensagemErro(player, "Você não está próximo a uma propriedade.");
    }

    [Command("campainha")]
    public void CMD_Campainha(Client player, int id)
    {
        var prop = recuperarPropriedadePorID(id);
        if (prop == null)
        {
            EnviarMensagemErro(player, "Propriedade inválida.");
            return;
        }

        var pos = new Vector3();
        if (prop.IDPropriedade == 0)
        {
            pos = prop.EntradaFrente;
        }
        else
        {
            var prop2 = recuperarPropriedadePorID(prop.IDPropriedade);
            pos = prop2.EntradaFrente;
        }

        var play = API.getPlayersInRadiusOfPosition(2, pos).Where(p => p.name.Equals(player.name)).ToList();
        if (play.Count == 0)
        {
            EnviarMensagemErro(player, "Você não está próximo a essa propriedade.");
            return;
        }

        foreach (var target in API.getPlayersInRadiusOfPosition(20, prop.SaidaFrente).Where(p => prop.ID.Equals(API.getEntitySyncedData(p, "idpropriedade"))))
            API.sendChatMessageToPlayer(target, COR_ROLEPLAY, "* A campainha está tocando.");

        EnviarMensagemSucesso(player, "Você tocou a campainha.");
        return;
    }

    [Command("olhomagico")]
    public void CMD_OlhoMagico(Client player)
    {
        foreach (var prop in propriedades)
        {
            if (prop.ID != API.getEntitySyncedData(player, "idpropriedade")) continue;

            var play = API.getPlayersInRadiusOfPosition(2, prop.SaidaFrente).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                var pos = new Vector3();
                if (prop.IDPropriedade == 0)
                {
                    pos = prop.EntradaFrente;
                }
                else
                {
                    var prop2 = recuperarPropriedadePorID(prop.IDPropriedade);
                    pos = prop2.EntradaFrente;
                }

                API.sendChatMessageToPlayer(player, COR_ROLEPLAY, "* Pessoas que podem ser vistas próximas a porta da frente *");
                foreach (var target in API.getPlayersInRadiusOfPosition(5, pos))
                    API.sendChatMessageToPlayer(player, string.Format("{0} com a skin {1}.", recuperarNomeIC(target), ((PedHash)target.model).ToString()));
                return;
            }
        }
        EnviarMensagemErro(player, "Você não está próximo a porta de saída de uma propriedade.");
    }
    #endregion

    #region Comandos Interpretação
    [Command("me", GreedyArg = true)]
    public void CMD_Me(Client player, string msg)
    {
        EnviarMensagemRadius(player, DISTANCIA_RP, string.Format("* {0} {1}", recuperarNomeIC(player), msg), COR_ROLEPLAY);
    }

    [Command("do", GreedyArg = true)]
    public void CMD_Do(Client player, string msg)
    {
        EnviarMensagemRadius(player, DISTANCIA_RP, string.Format("* {0} (({1}))", msg, recuperarNomeIC(player)), COR_ROLEPLAY);
    }
    #endregion


    [Command("comprar")]
    public void CMD_CompandoAlgo(Client player)
    {
        //Comprando casa
        foreach (var propLoop in propriedades)
        {
            var distancia_casa = API.getPlayersInRadiusOfPosition(2, propLoop.EntradaFrente).Where(p => p.name.Equals(player.name)).ToList();
            if (distancia_casa.Count > 0)
            {
                if (propLoop.IDPersonagemProprietario != 0)
                {
                    EnviarMensagemErro(player, "Essa propriedade não está a venda.");
                    return;
                }
                var index = propriedades.IndexOf(propLoop);

                var str = string.Empty;
                str = string.Format(@"SELECT Count(ID) ID FROM propriedades WHERE IDPersonagemProprietario = {0}", API.getEntitySyncedData(player, "id"));
                var cmd = new MySqlCommand(str, bancodados);
                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (isInt(dr["ID"].ToString()) >= MAX_PERSONAGENS_PROPRIEDADES)
                    {
                        EnviarMensagemErro(player, "Você atingiu o limite de propriedades por personagem.");
                        dr.Close();
                        return;
                    }
                }
                dr.Close();

                if (API.getEntitySyncedData(player, "dinheiro") < propLoop.ValorPropriedade)
                {
                    EnviarMensagemErro(player, "Dinheiro insuficiente.");
                    return;
                }
                API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - propLoop.ValorPropriedade);
                EnviarMensagemSucesso(player, "Propriedade adquirida.");
                var strUpdate = string.Format("UPDATE propriedades SET IDPersonagemProprietario = {0} WHERE ID = {1}", API.getEntitySyncedData(player, "id"), propLoop.ID);
                cmd = new MySqlCommand(strUpdate, bancodados);
                cmd.ExecuteNonQuery();

                propriedades[index].NomePersonagemProprietario = player.name;

                RecarregarPropriedade(player, propLoop.ID, "proprietario", string.Format("{0}", API.getEntitySyncedData(player, "id")));
                return;
            }
        }
        // /\ /\ /\ /\ /\ /\ Fim da compra de casa
    }
    [Command("trancar")]
    public void CMD_TrancandoAlgo(Client player)
    {
        //Casa
        foreach (var propLoop in propriedades)
        {

            var play = API.getPlayersInRadiusOfPosition(2, propLoop.SaidaFrente).Where(p => p.name.Equals(player.name)).ToList();
            var distancia_casa = API.getPlayersInRadiusOfPosition(2, propLoop.EntradaFrente).Where(p => p.name.Equals(player.name)).ToList();
            if (distancia_casa.Count > 0 || play.Count > 0)
            {

                if (propLoop.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                {
                    EnviarMensagemErro(player, "Você não tem as chaves desta porta.");
                    return;
                }

                var index = propriedades.IndexOf(propLoop);
                propriedades[index].StatusPortaFrente = !propriedades[index].StatusPortaFrente;
                if (propriedades[index].StatusPortaFrente)
                    EnviarMensagemSucesso(player, "Você abriu a porta da frente.");
                else
                    API.sendNotificationToPlayer(player, "~r~Você trancou a porta da frente.");

                return;

            }
        }
        // /\ /\ /\ /\ /\ /\ Fim da compra de casa
    }


    #region Comandos Propriedades
    [Command("p")]
    public void CMD_P(Client player, string opcao = "", int id = 0, string idOuNome = "", int preco = 0)
    {
        if (opcao.Trim().Equals(string.Empty))
        {
            var param1 = new List<string>();
            var param2 = new List<string>();

            param1.Add("Entrar");
            param2.Add("Entrar em Propriedades");

            if (API.getEntitySyncedData(player, "idpropriedade") != 0)
            {
                param1.Add("Sair");
                param2.Add("Sair de Propriedades");
            }

            API.triggerClientEvent(player, "menucomresposta", "INTERAÇÕES", "Lista de Interações", 6, param1, param2);
            return;
        }

        if (id == 0)
        {
            EnviarMensagemErro(player, "Insira o ID da residencia");
            return;
        }
        var prop = recuperarPropriedadePorID(id);

        if (prop == null)
        {
            EnviarMensagemErro(player, "Propriedade inválida.");
            return;
        }
        var index = propriedades.IndexOf(prop);
        var pos = new Vector3();
        var play = new List<Client>();
        var play2 = new List<Client>();

        var str = string.Empty;
        switch (opcao)
        {
            case "vender":
                var target = findPlayer(player, idOuNome);
                if (target == null)
                {
                    EnviarMensagemErro(player, "Jogador não encontrado.");
                    return;
                }

                str = string.Format(@"SELECT Count(ID) ID FROM propriedades WHERE IDPersonagemProprietario = {0}", API.getEntitySyncedData(target, "id"));
                var cmd5 = new MySqlCommand(str, bancodados);
                var dr5 = cmd5.ExecuteReader();

                if (dr5.Read())
                {
                    if (isInt(dr5["ID"].ToString()) >= MAX_PERSONAGENS_PROPRIEDADES)
                    {
                        EnviarMensagemErro(player, "O player alcançou o limite de propriedades.");
                        dr5.Close();
                        return;
                    }
                }
                dr5.Close();

                if (prop.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                {
                    EnviarMensagemErro(player, "Você não é o proprietário dessa propriedade.");
                    return;
                }

                if (prop.IDPropriedade == 0)
                {
                    play = API.getPlayersInRadiusOfPosition(2, prop.EntradaFrente).Where(p => p.name.Equals(player.name) || p.name.Equals(target.name)).ToList();
                }
                else
                {
                    var prop4 = recuperarPropriedadePorID(prop.IDPropriedade);
                    play = API.getPlayersInRadiusOfPosition(2, prop4.EntradaFrente).Where(p => p.name.Equals(player.name) || p.name.Equals(target.name)).ToList();
                }

                if (play.Count != 2)
                {
                    EnviarMensagemErro(player, "Você ou o comprador não está próximo da propriedade.");
                    return;
                }

                if (API.getEntitySyncedData(target, "dinheiro") < preco)
                {
                    EnviarMensagemErro(player, "O player não possui dinheiro suficiente.");
                    return;
                }

                API.setEntitySyncedData(target, "tipoconvite", 2);
                API.setEntitySyncedData(target, "idconvite", prop.ID);
                API.setEntitySyncedData(target, "idconvidador", recuperarIDPorClient(player));
                API.setEntitySyncedData(target, "extraconvite", preco);

                EnviarMensagemSucesso(player, string.Format("Você ofereceu sua propriedade para {0} por ${1}.", target.name, preco));
                EnviarMensagemSucesso(target, string.Format("{0} ofereceu uma propriedade por ${1}. (/aceitar prop ou /recusar prop)", player.name, preco));
                break;
            case "comprar":
                play = API.getPlayersInRadiusOfPosition(2, prop.EntradaFrente).Where(p => p.name.Equals(player.name)).ToList();
                if (play.Count > 0)
                {
                    if (prop.IDPersonagemProprietario != 0)
                    {
                        EnviarMensagemErro(player, "Essa propriedade não está a venda.");
                        return;
                    }

                    str = string.Format(@"SELECT Count(ID) ID FROM propriedades WHERE IDPersonagemProprietario = {0}", API.getEntitySyncedData(player, "id"));
                    var cmd = new MySqlCommand(str, bancodados);
                    var dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        if (isInt(dr["ID"].ToString()) >= MAX_PERSONAGENS_PROPRIEDADES)
                        {
                            EnviarMensagemErro(player, "Você atingiu o limite de propriedades por personagem.");
                            dr.Close();
                            return;
                        }
                    }
                    dr.Close();

                    if (API.getEntitySyncedData(player, "dinheiro") < prop.ValorPropriedade)
                    {
                        EnviarMensagemErro(player, "Dinheiro insuficiente.");
                        return;
                    }

                    API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - prop.ValorPropriedade);
                    EnviarMensagemSucesso(player, "Propriedade adquirida.");
                    var strUpdate = string.Format("UPDATE propriedades SET IDPersonagemProprietario = {0} WHERE ID = {1}", API.getEntitySyncedData(player, "id"), prop.ID);
                    cmd = new MySqlCommand(strUpdate, bancodados);
                    cmd.ExecuteNonQuery();

                    propriedades[index].NomePersonagemProprietario = player.name;

                    RecarregarPropriedade(player, prop.ID, "proprietario", string.Format("{0}", API.getEntitySyncedData(player, "id")));
                }
                else
                {
                    EnviarMensagemErro(player, "Você não está próximo a esta propriedade.");
                }
                break;
            case "abandonar":
                if (prop.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                {
                    EnviarMensagemErro(player, "Você não é o proprietário dessa propriedade.");
                    return;
                }

                if (prop.IDPropriedade == 0)
                {
                    pos = prop.EntradaFrente;
                }
                else
                {
                    var prop3 = recuperarPropriedadePorID(prop.IDPropriedade);
                    pos = prop3.EntradaFrente;
                }

                play = API.getPlayersInRadiusOfPosition(2, pos).Where(p => p.name.Equals(player.name)).ToList();

                if (play.Count == 0)
                {
                    EnviarMensagemErro(player, "Você não está próximo a sua propriedade.");
                    return;
                }

                EnviarMensagemSucesso(player, "Você abandonou a propriedade.");
                str = string.Format("UPDATE propriedades SET IDPersonagemProprietario = 0 WHERE ID = {0}", prop.ID);
                var cmd2 = new MySqlCommand(str, bancodados);
                cmd2.ExecuteNonQuery();

                RecarregarPropriedade(player, prop.ID, "proprietario", "0");
                break;
            case "trancar":
                if (prop.IDPropriedade == 0)
                {
                    pos = prop.EntradaFrente;
                }
                else
                {
                    var prop4 = recuperarPropriedadePorID(prop.IDPropriedade);
                    pos = prop4.EntradaFrente;
                }

                play = API.getPlayersInRadiusOfPosition(2, pos).Where(p => p.name.Equals(player.name)).ToList();
                play2 = API.getPlayersInRadiusOfPosition(2, prop.SaidaFrente).Where(p => p.name.Equals(player.name)).ToList();
                if (play.Count > 0 || play2.Count > 0)
                {
                    if (prop.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                    {
                        EnviarMensagemErro(player, "Você não possui a chave dessa porta.");
                        return;
                    }

                    propriedades[index].StatusPortaFrente = !propriedades[index].StatusPortaFrente;
                    if (propriedades[index].StatusPortaFrente)
                        EnviarMensagemSucesso(player, "Você abriu a porta da frente.");
                    else
                        API.sendNotificationToPlayer(player, "~r~Você trancou a porta da frente.");
                    return;
                }

                EnviarMensagemErro(player, "Você não está próximo a nenhuma propriedade.");
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                break;
        }
    }
    #endregion

    #region Comandos Interações
    [Command("entrar")]
    public void CMD_Entrar(Client player)
    {
        InteracaoEntrar(player);
    }

    [Command("sair")]
    public void CMD_Sair(Client player)
    {
        InteracaoSair(player);
    }

    [Command("motor")]
    public void CMD_Motor(Client player)
    {
        MotorVeiculo(player);
    }
    #endregion

    #region Comandos Veiculares
    [Command("portamalas")]
    public void CMD_portamalas(Client player, string acao = "", string opcao = "", int slot = 0, int quantidade = 0)
    {
        if (acao == "")
        {
            EnviarMensagemErro(player, "USE: /portamalas [acao]\nAções: abrir, fechar, ver, colocar, pegar"); return;
        }
        foreach (var veh in API.getAllVehicles())
        {
            var play = API.getPlayersInRadiusOfPosition(3, API.getEntityPosition(veh)).Where(p => p.name.Equals(player.name)).ToList();
            if (play.Count > 0)
            {
                if (API.getEntitySyncedData(player, "id") != recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id")).IDPersonagemProprietario)
                {
                    EnviarMensagemErro(player, "Você não possui a chave desse veículo.");
                    return;
                }

                var car = recuperarVeiculoPorID(API.getEntitySyncedData(veh, "id"));

                switch (acao)
                {
                    case "abrir":

                        if (API.getVehicleLocked(veh))
                        {
                            EnviarMensagemErro(player, "O veículo está trancado.");
                            return;
                        }

                        API.setVehicleDoorState(veh, 5, true);
                        break;
                    case "fechar":
                        API.setVehicleDoorState(veh, 5, false);
                        break;
                    case "ver":
                        if (!API.getVehicleDoorState(veh, 5))
                        {
                            EnviarMensagemErro(player, "O portamalas não está aberto.");
                            return;
                        }

                        API.sendChatMessageToPlayer(player, "====== [PORTA-MALAS: " + car.Modelo + "] ======");
                        API.sendChatMessageToPlayer(player, "1: " + car.Inv1 + " (" + car.Inv1_q + ") | 2: " + car.Inv2 + " (" + car.Inv2_q + ")  | 3: " + car.Inv3 + " (" + car.Inv3_q + ")  | 4: " + car.Inv4 + " (" + car.Inv4_q + ") | 5: " + car.Inv5 + " (" + car.Inv5_q + ")");
                        API.sendChatMessageToPlayer(player, "6: " + car.Inv6 + " (" + car.Inv6_q + ") | 7: " + car.Inv7 + " (" + car.Inv7_q + ")  | 8: " + car.Inv8 + " (" + car.Inv8_q + ")  | 9: " + car.Inv9 + " (" + car.Inv9_q + ") | 10: " + car.Inv10 + " (" + car.Inv10_q + ")");
                        break;
                    case "colocar":
                        if (!API.getVehicleDoorState(veh, 5))
                        {
                            EnviarMensagemErro(player, "O portamalas não está aberto.");
                            return;
                        }
                        if (opcao == "") { EnviarMensagemErro(player, "Selecione uma opção: 'arma' ou 'dinheiro'.\n/portamalas [colocar] [arma] [slot]"); return; }
                        if (slot == 0) { EnviarMensagemErro(player, "O slot deve ser entre 1 e 10.\n/portamalas [colocar] [arma] [slot]"); return; }
                        if (opcao == "arma")
                        {
                            if (slot < 1 && slot > 10) { EnviarMensagemErro(player, "O slot deve ser entre 1 e 10."); return; }
                            switch (slot)
                            {
                                case 1:
                                    car.Inv1 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv1_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:1] Você guardou um(a) " + car.Inv1 + " com " + car.Inv1_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 2:
                                    car.Inv2 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv2_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:2] Você guardou um(a) " + car.Inv2 + " com " + car.Inv2_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 3:
                                    car.Inv3 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv3_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:3] Você guardou um(a) " + car.Inv3 + " com " + car.Inv3_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 4:
                                    car.Inv4 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv4_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:4] Você guardou um(a) " + car.Inv4 + " com " + car.Inv4_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 5:
                                    car.Inv5 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv5_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:5] Você guardou um(a) " + car.Inv5 + " com " + car.Inv5_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 6:
                                    car.Inv6 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv6_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:6] Você guardou um(a) " + car.Inv6 + " com " + car.Inv6_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 7:
                                    car.Inv7 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv7_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:7] Você guardou um(a) " + car.Inv7 + " com " + car.Inv7_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 8:
                                    car.Inv8 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv8_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:8] Você guardou um(a) " + car.Inv8 + " com " + car.Inv8_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 9:
                                    car.Inv9 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv9_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:9] Você guardou um(a) " + car.Inv9 + " com " + car.Inv9_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                                case 10:
                                    car.Inv10 = API.getPlayerCurrentWeapon(player).ToString();
                                    car.Inv10_q = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                                    API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                                    EnviarMensagemSucesso(player, "[S:10] Você guardou um(a) " + car.Inv10 + " com " + car.Inv10_q + " balas no portamalas do seu " + car.Modelo + ".");
                                    break;
                            }
                            SalvarPortaMalas(car.ID);
                        }
                        break;
                    case "pegar":
                        if (!API.getVehicleDoorState(veh, 5))
                        {
                            EnviarMensagemErro(player, "O portamalas não está aberto.");
                            return;
                        }
                        if (API.getPlayerCurrentWeapon(player).ToString() != "Unarmed")
                        {
                            EnviarMensagemErro(player, "Você já tem uma arma em sua mão.");
                            return;
                        }
                        if (opcao == "") { EnviarMensagemErro(player, "Selecione uma opção: 'arma' ou 'dinheiro'.\n/portamalas [pegar] [arma] [slot]"); return; }
                        if (slot == 0) { EnviarMensagemErro(player, "O slot deve ser entre 1 e 10.\n/portamalas [pegar] [arma] [slot]"); return; }
                        if (opcao == "arma")
                        {
                            if (slot < 1 && slot > 10) { EnviarMensagemErro(player, "O slot deve ser entre 1 e 10."); return; }
                            switch (slot)
                            {
                                case 1:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv1), car.Inv1_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:1] Você pegou um(a) " + car.Inv1 + " com " + car.Inv1_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv1 = string.Empty;
                                    car.Inv1_q = 0;
                                    break;
                                case 2:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv2), car.Inv2_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:2] Você pegou um(a) " + car.Inv2 + " com " + car.Inv2_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv2 = string.Empty;
                                    car.Inv2_q = 0;
                                    break;
                                case 3:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv3), car.Inv3_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:3] Você pegou um(a) " + car.Inv3 + " com " + car.Inv3_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv3 = string.Empty;
                                    car.Inv3_q = 0;
                                    break;
                                case 4:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv4), car.Inv3_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:4] Você pegou um(a) " + car.Inv4 + " com " + car.Inv4_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv4 = string.Empty;
                                    car.Inv4_q = 0;
                                    break;
                                case 5:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv5), car.Inv5_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:5] Você pegou um(a) " + car.Inv5 + " com " + car.Inv5_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv5 = string.Empty;
                                    car.Inv5_q = 0;
                                    break;
                                case 6:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv6), car.Inv6_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:6] Você pegou um(a) " + car.Inv6 + " com " + car.Inv6_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv6 = string.Empty;
                                    car.Inv6_q = 0;
                                    break;
                                case 7:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv7), car.Inv7_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:7] Você pegou um(a) " + car.Inv7 + " com " + car.Inv7_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv7 = string.Empty;
                                    car.Inv7_q = 0;
                                    break;
                                case 8:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv8), car.Inv8_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:8] Você pegou um(a) " + car.Inv8 + " com " + car.Inv8_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv8 = string.Empty;
                                    car.Inv8_q = 0;
                                    break;
                                case 9:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv9), car.Inv9_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:9] Você pegou um(a) " + car.Inv9 + " com " + car.Inv9_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv9 = string.Empty;
                                    car.Inv9_q = 0;
                                    break;
                                case 10:
                                    API.givePlayerWeapon(player, API.weaponNameToModel(car.Inv10), car.Inv10_q, true, true);
                                    EnviarMensagemSucesso(player, "[S:10] Você pegou um(a) " + car.Inv10 + " com " + car.Inv10_q + " balas no portamalas do seu " + car.Veh + ".");

                                    car.Inv10 = string.Empty;
                                    car.Inv10_q = 0;
                                    break;
                            }
                            SalvarPortaMalas(car.ID);

                        }
                        break;
                }
                return;
            }
        }
    }

    [Command("v")]
    public void CMD_V(Client player, string opcao = "", string item = "", int preco = 0)
    {
        if (opcao.Trim().Equals(string.Empty))
        {
            if (!player.isInVehicle)
            {
                EnviarMensagemErro(player, "Você não está em um veículo.");
                return;
            }

            var param1 = new List<string>();
            var param2 = new List<string>();

            if (player.vehicleSeat == -1)
            {
                param1.Add("Motor");
                param2.Add("Ligar / Desligar Motor");
            }

            param1.Add("Porta-Malas");
            param1.Add("Capô");
            param1.Add("Porta Dianteira Esquerda");
            param1.Add("Porta Dianteira Direita");
            param1.Add("Porta Traseira Esquerda");
            param1.Add("Porta Traseira Direita");

            param2.Add("Abrir / Trancar Porta-Malas do Veículo");
            param2.Add("Abrir / Trancar Capô do Veículo");
            param2.Add("Abrir / Trancar Porta");
            param2.Add("Abrir / Trancar Porta");
            param2.Add("Abrir / Trancar Porta");
            param2.Add("Abrir / Trancar Porta");

            API.triggerClientEvent(player, "menucomresposta", "INTERAÇÕES", "Lista de Interações", 6, param1, param2);
            return;
        }

        var str = string.Empty;
        switch (opcao)
        {
            case "lista":
                var param1 = new List<string>();
                var param2 = new List<string>();

                str = string.Format("SELECT * FROM veiculos WHERE IDPersonagemProprietario={0} ORDER BY ModeloVeiculo", API.getEntitySyncedData(player, "id"));
                var cmd = new MySqlCommand(str, bancodados);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var spawn = "~g~SPAWNADO";
                    if (recuperarVeiculoPorID(isInt(dr["ID"].ToString())) == null)
                        spawn = "~r~DESPAWNADO";
                    if (isInt(dr["explodido"].ToString()) == 1)
                        spawn = "~r~EXPLODIDO";

                    param1.Add(dr["ModeloVeiculo"].ToString());
                    param2.Add(string.Format("ID: {0} {1}", dr["ID"].ToString(), spawn));
                }
                dr.Close();

                API.triggerClientEvent(player, "menusemresposta", "VEÍCULOS", "Listagem de Veículos", 6, param1, param2);
                break;
            case "comprar":
                str = string.Format(@"SELECT Count(ID) ID FROM veiculos WHERE IDPersonagemProprietario = {0}", API.getEntitySyncedData(player, "id"));
                var cmd4 = new MySqlCommand(str, bancodados);
                var dr4 = cmd4.ExecuteReader();

                if (dr4.Read())
                {
                    if (isInt(dr4["ID"].ToString()) >= MAX_PERSONAGENS_VEICULOS)
                    {
                        EnviarMensagemErro(player, "Você atingiu o limite de veículos por personagem.");
                        dr4.Close();
                        return;
                    }
                }
                dr4.Close();

                foreach (var conce in concessionarias)
                {
                    var ply = API.getPlayersInRadiusOfPosition(3, conce.Posicao).Where(pl => pl.name.Equals(player.name)).ToList();
                    if (ply.Count > 0)
                    {
                        var list = new List<Dictionary<string, object>>();
                        var vehs = new List<Concessionaria.Veiculo>();

                        if (conce.Tipo.Equals("Sedans"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Sedans")).ToList();
                        else if (conce.Tipo.Equals("Boats"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Boats")).ToList();
                        else if (conce.Tipo.Equals("Commercials"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Commercials")).ToList();
                        else if (conce.Tipo.Equals("Compacts"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Compacts")).ToList();
                        else if (conce.Tipo.Equals("Off-Road"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Off-Road")).ToList();
                        else if (conce.Tipo.Equals("Planes"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Planes")).ToList();
                        else if (conce.Tipo.Equals("Sports"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Sports")).ToList();
                        else if (conce.Tipo.Equals("SUVs"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("SUVs")).ToList();
                        else if (conce.Tipo.Equals("Service"))
                            vehs = veiculosConce.Where(v => v.Categoria.Equals("Service")).ToList();

                        API.setEntitySyncedData(player, "ConceID", conce.ID);

                        foreach (var veh in vehs)
                        {
                            var dic = new Dictionary<string, object>();
                            var v = API.vehicleNameToModel(veh.Nome);

                            dic["veiculo"] = veh.Nome;
                            dic["preco"] = veh.Preco;
                            dic["lugares"] = API.getVehicleMaxOccupants(v);
                            dic["velocidade"] = veh.Velocidade;
                            dic["frenagem"] = veh.Frenagem;
                            dic["aceleracao"] = veh.Aceleracao;
                            dic["tracao"] = veh.Tracao;

                            var geral = (float)(veh.Velocidade + veh.Frenagem + veh.Aceleracao + veh.Tracao);
                            dic["geral"] = geral.Equals(0) ? 0 : geral / 4;

                            list.Add(dic);
                        }

                        API.triggerClientEvent(player, "conce", API.toJson(list));
                        return;
                    }
                }

                EnviarMensagemErro(player, "Você não está em uma concessionária.");
                break;
            case "trancar":
                InteracaoAbrirTrancar(player);
                break;
            case "capo":
                InteracaoAbrirTrancarCapo(player);
                break;
            case "portamalas":
                InteracaoAbrirTrancarPortaMalas(player);
                break;
            case "porta":
                if (isInt(item) < 1 || isInt(item) > 4)
                {
                    EnviarMensagemErro(player, "A porta deve ser entre 1 e 4.");
                    return;
                }

                InteracaoAbrirTrancarPortaVeh(player, isInt(item) - 1);
                break;
            case "remontar":

                var distance_p = API.getPlayersInRadiusOfPosition(3, new Vector3(-202.2094, -1158.326, 23.81366)).Where(pl => pl.name.Equals(player.name)).ToList();
                if (distance_p.Count > 0)
                {
                    if (isInt(item) <= 0)
                    {
                        EnviarMensagemErro(player, "O ID do veículo precisa ser maior que 0.");
                        return;
                    }
                    if (recuperarVeiculoPorID(isInt(item)) != null)
                    {
                        EnviarMensagemErro(player, "Este veículo está spawnado.");
                        return;
                    }

                    var string_remontar = string.Format("SELECT * FROM veiculos WHERE ID={0} LIMIT 1", item);
                    var sql = new MySqlCommand(string_remontar, bancodados);
                    var car = sql.ExecuteReader();
                    if (car.Read())
                    {

                        if (isInt(car["IDPersonagemProprietario"].ToString()) != API.getEntitySyncedData(player, "id"))
                        {
                            EnviarMensagemErro(player, "Você não é o proprietário deste veículo.");
                            car.Close();
                            return;
                        }
                        if (isInt(car["explodido"].ToString()) == 0)
                        {
                            EnviarMensagemErro(player, "Este veículo não está explodido.");
                            car.Close();
                            return;
                        }
                        if (API.getEntitySyncedData(player, "dinheiro") < 500)
                        {
                            EnviarMensagemErro(player, "Você não tem $500 para recuperar o veículo.");
                            car.Close();
                            return;
                        }

                        API.setEntitySyncedData(player, "dinheiro", API.getEntitySyncedData(player, "dinheiro") - 500);
                        SpawnarVeiculo(car, player, 1);
                    }
                    car.Close();

                    var str_desexplodir = string.Format("UPDATE veiculos SET explodido=0 WHERE ID={0}", item);
                    var cmd_des = new MySqlCommand(str_desexplodir, bancodados);
                    cmd_des.ExecuteNonQuery();



                }
                else
                    EnviarMensagemErro(player, "Você está longe da Mors Mutual Seguros. (Utilize o GPS se preciso.)");

                break;
            case "spawn":
                if (isInt(item) <= 0)
                {
                    EnviarMensagemErro(player, "O ID do veículo precisa ser maior que 0.");
                    return;
                }

                if (recuperarVeiculoPorID(isInt(item)) != null)
                {
                    EnviarMensagemErro(player, "Este veículo já está spawnado.");
                    return;
                }

                str = string.Format("SELECT * FROM veiculos WHERE IDPersonagemProprietario={0} AND ID={1}", API.getEntitySyncedData(player, "id"), item);
                var cmd2 = new MySqlCommand(str, bancodados);
                var dr2 = cmd2.ExecuteReader();
                if (dr2.HasRows)
                {
                    dr2.Read();
                    SpawnarVeiculo(dr2, player);
                }
                else
                {
                    EnviarMensagemErro(player, "Esse veículo não pertence a você.");
                }
                dr2.Close();

                break;
            /*case "estacionar":
                if (!player.isInVehicle || player.vehicleSeat != -1)
                {
                    EnviarMensagemErro(player, "Você não está no banco do motorista de um veículo.");
                    return;
                }

                var veh2 = recuperarVeiculo(player.vehicle);
                if (veh2.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                {
                    EnviarMensagemErro(player, "Você não é o proprietário deste veículo.");
                    return;
                }

                var play = API.getPlayersInRadiusOfPosition(3, veh2.PosicaoSpawn).Where(p => p.name.Equals(player.name)).ToList();
                if (play.Count == 0)
                {
                    EnviarMensagemErro(player, "Você não está próximo à sua vaga de estacionamento.");
                    return;
                }

                str = string.Format("UPDATE veiculos SET Vida={0} WHERE ID={1}", API.getVehicleHealth(player.vehicle), veh2.ID);
                var cmd3 = new MySqlCommand(str, bancodados);
                cmd3.ExecuteNonQuery();

                veh2.Veh.delete();
                veiculos.Remove(veh2);

                EnviarMensagemSucesso(player, "Você estacionou seu veículo.");
                break;*/
            case "vender":
                if (item.Trim().Equals(string.Empty))
                {
                    EnviarMensagemErro(player, "Jogador não informado.");
                    return;
                }

                var target = findPlayer(player, item);
                if (target == null)
                {
                    EnviarMensagemErro(player, "Jogador não encontrado.");
                    return;
                }

                str = string.Format(@"SELECT Count(ID) ID FROM veiculos WHERE IDPersonagemProprietario = {0}", API.getEntitySyncedData(target, "id"));
                var cmd5 = new MySqlCommand(str, bancodados);
                var dr5 = cmd5.ExecuteReader();

                if (dr5.Read())
                {
                    if (isInt(dr5["ID"].ToString()) >= MAX_PERSONAGENS_VEICULOS)
                    {
                        EnviarMensagemErro(player, "O player alcançou o limite de veículos.");
                        dr5.Close();
                        return;
                    }
                }
                dr5.Close();

                var veh3 = recuperarVeiculo(player.vehicle);
                if (veh3.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                {
                    EnviarMensagemErro(player, "Você não é o proprietário desse veículo.");
                    return;
                }

                var play = API.getPlayersInRadiusOfPosition(2, API.getEntityPosition(player.vehicle)).Where(p => p.name.Equals(player.name) || p.name.Equals(target.name)).ToList();
                if (play.Count != 2)
                {
                    EnviarMensagemErro(player, "O player não está próximo do veículo.");
                    return;
                }

                if (API.getEntitySyncedData(target, "dinheiro") < preco)
                {
                    EnviarMensagemErro(player, "O player não possui dinheiro suficiente.");
                    return;
                }

                API.setEntitySyncedData(target, "tipoconvite", 3);
                API.setEntitySyncedData(target, "idconvite", veh3.ID);
                API.setEntitySyncedData(target, "idconvidador", recuperarIDPorClient(player));
                API.setEntitySyncedData(target, "extraconvite", preco);

                EnviarMensagemSucesso(player, string.Format("Você ofereceu seu veículo para {0} por ${1}.", target.name, preco));
                EnviarMensagemSucesso(target, string.Format("{0} ofereceu um veículo por ${1}. (/aceitar veiculo ou /recusar veiculo)", player.name, preco));
                break;
            case "comprarvaga":
                /*var veh4 = recuperarVeiculo(player.vehicle);
                if (veh4.IDPersonagemProprietario != API.getEntitySyncedData(player, "id"))
                {
                    EnviarMensagemErro(player, "Você não é o proprietário deste veículo.");
                    return;
                }

                if (player.vehicle.engineStatus)
                {
                    EnviarMensagemErro(player, "O motor do veículo deve estar desligado.");
                    return;
                }

                var perto_casa = 0;
                foreach (var prop in propriedades)
                {
                    if (prop.IDPersonagemProprietario == API.getEntitySyncedData(player, "id"))
                    {
                        var casas_prox = API.getPlayersInRadiusOfPosition(60, prop.EntradaFrente).Where(p => p.name.Equals(player.name)).ToList();
                        if (casas_prox.Count > 0)
                        {
                            perto_casa = 1;
                            break;
                        }
                    }
                }

                if (perto_casa == 1)
                {
                    var strUpdate = string.Format("UPDATE veiculos SET PosX={0},PosZ={1},PosY={2},RotX={3},RotZ={4},RotY={5},Dimensao={6} WHERE ID={7}",
                        aFlt(player.vehicle.position.X),
                        aFlt(player.vehicle.position.Z),
                        aFlt(player.vehicle.position.Y),
                        aFlt(player.vehicle.rotation.X),
                        aFlt(player.vehicle.rotation.Z),
                        aFlt(player.vehicle.rotation.Y),
                        player.dimension,
                        veh4.ID);
                    var cmd6 = new MySqlCommand(strUpdate, bancodados);
                    cmd6.ExecuteNonQuery();

                    EnviarMensagemSucesso(player, string.Format("Você estacionou seu veículo aqui."));
                }
                else
                {
                    EnviarMensagemErro(player, string.Format("Você não está próximo de nenhuma casa sua."));
                }*/
                EnviarMensagemSucesso(player, "O seu carro será salvo no local em que você deixa-lo..");
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                break;
        }
    }
    #endregion

    #region Comandos Administrativos

    #region Tester (1)
    [Command("kick", GreedyArg = true)]
    public void CMD_Kick(Client player, string idOrName, string motivo)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getEntitySyncedData(player, "staff") <= API.getEntitySyncedData(target, "staff"))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var log = string.Format("{0} kickou {1} ({2})", API.getEntitySyncedData(player, "usuario"), target.name.Replace("_", " "), motivo);
        AdicionarPunicao(player, target, "kick", motivo);
        API.kickPlayer(target, motivo);
        EnviarMensagemSucesso(player, string.Format("Você kickou {0}.", target.name.Replace("_", " ")));
        CriarLog(LOG_KICKS, log);
        EnviarMensagemStaff(1, log);
    }

    [Command("ir")]
    public void CMD_Ir(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        var pos = target.position;
        pos.X = pos.X + 3;
        API.setEntityPosition(player, pos);
        API.setEntityDimension(player, API.getEntityDimension(target));

        EnviarMensagemSucesso(player, string.Format("Você foi até {0}.", target.name.Replace("_", " ")));
    }

    [Command("trazer")]
    public void CMD_Trazer(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getEntitySyncedData(player, "staff") <= API.getEntitySyncedData(target, "staff"))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pos = player.position;
        pos.X = pos.X + 3;
        API.setEntityPosition(target, pos);
        API.setEntityDimension(target, API.getEntityDimension(player));

        EnviarMensagemSucesso(player, string.Format("Você trouxe {0}.", target.name.Replace("_", " ")));
        EnviarMensagemSucesso(target, string.Format("{0} puxou você.", API.getEntitySyncedData(player, "usuario")));
    }

    [Command("aj", GreedyArg = true)]
    public void CMD_AJ(Client player, int id, string msg)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var sos = recuperarSOSPorID(id);
        if (sos == null)
        {
            EnviarMensagemErro(player, "Não há SOS para o ID indicado.");
            return;
        }

        if (sos.NomePersonagem == player.name)
        {
            EnviarMensagemErro(player, "Você não pode aceitar seu próprio SOS.");
            return;
        }

        var target = recuperarClientPorID(sos.IDPlayer);
        if (target == null)
        {
            listaSOS.Remove(sos);
            EnviarMensagemErro(player, "O player saiu do servidor.");
            return;
        }

        if (sos.NomePersonagem != target.name)
        {
            listaSOS.Remove(sos);
            EnviarMensagemErro(player, "O player saiu do servidor.");
            return;
        }

        EnviarMensagemSucesso(player, string.Format("Você aceitou o SOS de {0} [{1}].", target.name.Replace("_", " "), sos.IDPlayer));
        EnviarMensagemStaff(1, string.Format("{2} aceitou o SOS de {0} [{1}].",
            target.name.Replace("_", " "),
            sos.IDPlayer,
            API.getEntitySyncedData(player, "usuario")));
        EnviarMensagemSucesso(target, string.Format("{0} aceitou seu SOS.", API.getEntitySyncedData(player, "usuario")));
        EnviarPMStaff(player, target, msg);

        API.setEntitySyncedData(player, "quantidadesos", API.getEntitySyncedData(player, "quantidadesos") + 1);

        listaSOS.Remove(sos);
    }

    [Command("rj", GreedyArg = true)]
    public void CMD_RJ(Client player, int id, string msg)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var sos = recuperarSOSPorID(id);
        if (sos == null)
        {
            EnviarMensagemErro(player, "Não há SOS para o ID indicado.");
            return;
        }

        var target = recuperarClientPorID(sos.IDPlayer);
        if (target == null)
        {
            listaSOS.Remove(sos);
            EnviarMensagemErro(player, "O player saiu do servidor.");
            return;
        }

        if (sos.NomePersonagem != target.name)
        {
            listaSOS.Remove(sos);
            EnviarMensagemErro(player, "O player saiu do servidor.");
            return;
        }

        EnviarMensagemSucesso(player, string.Format("Você rejeitou o SOS de {0} [{1}].", target.name.Replace("_", " "), sos.IDPlayer));
        EnviarMensagemStaff(1, string.Format("{2} rejeitou o SOS de {0} [{1}].",
            target.name.Replace("_", " "),
            sos.IDPlayer,
            API.getEntitySyncedData(player, "usuario")));
        API.sendNotificationToPlayer(target, string.Format("~r~{0} rejeitou seu SOS.", API.getEntitySyncedData(player, "usuario")));
        EnviarPMStaff(player, target, msg);

        listaSOS.Remove(sos);
    }

    [Command("listarsos")]
    public void CMD_ListarSOS(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.triggerClientEvent(player, "listarsos", API.toJson(listaSOS), listaSOS.Count.ToString());
    }

    [Command("apm", GreedyArg = true)]
    public void CMD_APM(Client player, string idOrName, string msg)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }
        if (player == target)
        {
            EnviarMensagemErro(player, "Você não pode enviar uma APM para si mesmo.");
            return;
        }

        EnviarPMStaff(player, target, msg);
    }

    [Command("atrabalho")]
    public void CMD_ATrabalho(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 1)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.setEntitySyncedData(player, "atrabalho", !API.getEntitySyncedData(player, "atrabalho"));

        if (API.getEntitySyncedData(player, "atrabalho"))
        {
            if (API.getEntitySyncedData(player, "staff") == 1)
                API.setPlayerNametagColor(player, 255, 231, 92);
            else
                API.setPlayerNametagColor(player, 92, 201, 255);

            EnviarMensagemSucesso(player, "Você entrou em trabalho administrativo.");
        }
        else
        {
            API.setPlayerNametagColor(player, 255, 255, 255);
            API.sendNotificationToPlayer(player, "~r~Você saiu do trabalho administrativo.");
        }
    }
    #endregion

    #region Game Admin 1 (2)
    [Command("a", GreedyArg = true)]
    public void CMD_A(Client player, string msg)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (API.getEntitySyncedData(player, "togchatstaff"))
        {
            EnviarMensagemErro(player, "Você desabilitou o chat da staff.");
            return;
        }

        EnviarMensagemChatStaff(string.Format("(( {0} {1}: {2} ))", recuperarNomeStaff(API.getEntitySyncedData(player, "staff")), API.getEntitySyncedData(player, "usuario"), msg));
    }

    [Command("reviver")]
    public void CMD_Reviver(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (!API.getEntitySyncedData(target, "ferido"))
        {
            EnviarMensagemErro(player, "Jogador não está brutalmente ferido.");
            return;
        }

        ReviverPlayer(target);
        API.sendNotificationToPlayer(player, string.Format("~g~Você reviveu {0}.", target.name.Replace("_", " ")));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} reviveu você.", API.getEntitySyncedData(player, "usuario")));
    }

    [Command("checaratirador")]
    public void CMD_ChecarAtirador(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (!API.hasEntitySyncedData(target, "playerkiller"))
        {
            EnviarMensagemErro(player, "Jogador não foi morto por ninguém.");
            return;
        }

        API.sendNotificationToPlayer(player, string.Format("{0} foi morto por {1}.", target.name.Replace("_", " "), API.getEntitySyncedData(target, "playerkiller")));
    }

    [Command("vida")]
    public void CMD_Vida(Client player, string idOrName, int vida)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (vida < 1 || vida > 100)
        {
            EnviarMensagemErro(player, "Vida precisa ser entre 1 e 100.");
            return;
        }

        API.setPlayerHealth(target, vida);
        API.sendNotificationToPlayer(player, string.Format("~g~Você modificou a vida de {0} para {1}.", target.name.Replace("_", " "), vida));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} modificou sua vida para {1}.", API.getEntitySyncedData(player, "usuario"), vida));
    }

    [Command("congelar")]
    public void CMD_Congelar(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        API.freezePlayer(target, true);
        API.sendNotificationToPlayer(player, string.Format("~g~Você congelou {0}.", target.name.Replace("_", " ")));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} congelou você.", API.getEntitySyncedData(player, "usuario")));
    }

    [Command("descongelar")]
    public void CMD_Descongelar(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        API.freezePlayer(target, false);
        API.sendNotificationToPlayer(player, string.Format("~g~Você descongelou {0}.", target.name.Replace("_", " ")));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} descongelou você.", API.getEntitySyncedData(player, "usuario")));
    }

    [Command("skin")]
    public void CMD_Skin(Client player, string idOrName, string skin)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        var sk = API.pedNameToModel(skin);
        if (sk == 0)
        {
            EnviarMensagemErro(player, "A skin informada não existe.");
            return;
        }

        var s = SKINS_PROIBIDAS_SET.Where(ski => ski.ToLower().Equals(skin.ToLower())).ToList();
        if (s.Count > 0)
        {
            EnviarMensagemErro(player, "Você não pode setar essa skin.");
            return;
        }

        var weaponData = new Dictionary<WeaponHash, CWeaponData>();
        foreach (WeaponHash wepHash in API.getPlayerWeapons(target))
            weaponData.Add(wepHash,
                new CWeaponData
                {
                    Ammo = API.getPlayerWeaponAmmo(target, wepHash),
                    Tint = API.getPlayerWeaponTint(target, wepHash),
                    Components = JsonConvert.SerializeObject(API.getPlayerWeaponComponents(target, wepHash))
                });

        API.setPlayerSkin(target, sk);

        foreach (var weapon in weaponData)
        {
            API.givePlayerWeapon(target, weapon.Key, weapon.Value.Ammo, false, true);
            API.setPlayerWeaponTint(target, weapon.Key, weapon.Value.Tint);

            var weaponMods = JsonConvert.DeserializeObject<List<WeaponComponent>>(weapon.Value.Components);
            foreach (WeaponComponent compID in weaponMods) API.givePlayerWeaponComponent(target, weapon.Key, compID);
        }

        API.sendNotificationToPlayer(player, string.Format("~g~Você alterou a skin de {0} para {1}.", target.name.Replace("_", " "), sk.ToString()));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} alterou sua skin para {1}.", API.getEntitySyncedData(player, "usuario"), sk.ToString()));
    }

    [Command("spec")]
    public void CMD_Spec(Client player, string idOrName)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        API.setEntityDimension(player, API.getEntityDimension(player));
        API.setPlayerToSpectatePlayer(player, target);
        EnviarMensagemSucesso(player, string.Format("Você está espiando {0}.", target.name.Replace("_", " ")));
    }

    [Command("unspec")]
    public void CMD_Unspec(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.unspectatePlayer(player);
        API.setEntityDimension(player, 0);
        EnviarMensagemSucesso(player, "Você parou de espiar.");
    }

    [Command("teleportar")]
    public void CMD_Teleportar(Client player, string idOrName, string idOrName2)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Player 1 não encontrado.");
            return;
        }

        var target2 = findPlayer(player, idOrName2);
        if (target2 == null)
        {
            EnviarMensagemErro(player, "Player 2 não encontrado.");
            return;
        }

        if (target == target2)
        {
            EnviarMensagemErro(player, "Os players precisam ser diferentes.");
            return;
        }

        if (API.getEntitySyncedData(player, "staff") <= API.getEntitySyncedData(target, "staff"))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pos = target2.position;
        pos.X = pos.X + 3;
        API.setEntityPosition(target, pos);
        API.setEntityDimension(target, API.getEntityDimension(target2));

        EnviarMensagemSucesso(player, string.Format("Você teleportou {0} para {1}.", target.name.Replace("_", " "), target2.name.Replace("_", " ")));
        EnviarMensagemSucesso(target, string.Format("{0} teleportou você para {1}.", API.getEntitySyncedData(player, "usuario"), target2.name.Replace("_", " ")));
    }

    [Command("ban", GreedyArg = true)]
    public void CMD_Ban(Client player, string idOrName, string motivo)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getEntitySyncedData(player, "staff") <= API.getEntitySyncedData(target, "staff"))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var log = string.Format("{0} baniu {1}. ({2})", API.getEntitySyncedData(player, "usuario"), target.name.Replace("_", " "), motivo);

        CriarLog(LOG_PUNICOES, log);
        AdicionarBanimento(player, target, motivo);
        AdicionarPunicao(player, target, "ban", motivo);
        EnviarMensagemStaff(2, log);
        API.kickPlayer(target, "Você foi banido.");
        EnviarMensagemSucesso(player, string.Format("Você baniu {0}.", target.name.Replace("_", " ")));
    }

    [Command("bantemp", "~y~USO: ~w~/bantemp [idOrName] [d ou h] [dias ou horas] [motivo]", GreedyArg = true)]
    public void CMD_BanTemp(Client player, string idOrName, string tipo, int duracao, string motivo)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getEntitySyncedData(player, "staff") <= API.getEntitySyncedData(target, "staff"))
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (!(tipo == "d" || tipo == "h"))
        {
            EnviarMensagemErro(player, "O tipo deve ser 'd' para dias ou 'h' para horas.");
            return;
        }

        if (duracao <= 0)
        {
            EnviarMensagemErro(player, "A duração precisa ser maior que 0.");
            return;
        }

        if (tipo == "d")
            duracao = duracao * 24;

        var log = string.Format("{0} baniu temporariamente {1}. ({2})", API.getEntitySyncedData(player, "usuario"), target.name.Replace("_", " "), motivo);

        CriarLog(LOG_PUNICOES, log);
        AdicionarBanimento(player, target, motivo, duracao);
        AdicionarPunicao(player, target, "bantemp", motivo, duracao);
        EnviarMensagemStaff(2, log);
        API.kickPlayer(target, "Você foi banido temporariamente.");
        EnviarMensagemSucesso(player, string.Format("Você baniu {0} temporariamente.", target.name.Replace("_", " ")));
    }

    [Command("unban")]
    public void CMD_UnBan(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var str = string.Format("DELETE FROM banimentos WHERE ID = {0}; DELETE FROM tentativasburlarban WHERE IDBanimento = {0};", id);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

        EnviarMensagemSucesso(player, string.Format("Caso haja um banimento com ID {0}, ele foi removido com sucesso.", id));
        CriarLog(LOG_UNBAN, string.Format("{0} removeu o banimento {1}", API.getEntitySyncedData(player, "usuario"), id));
    }

    [Command("proximo")]
    public void CMD_Proximo(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.sendChatMessageToPlayer(player, "~#8CE2FF~", "Itens Próximos");

        foreach (var arm in armarios)
        {
            var itens = API.getPlayersInRadiusOfPosition(10, arm.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
                API.sendChatMessageToPlayer(player, string.Format("Armário {0}", arm.ID));
        }

        foreach (var blip in blips)
        {
            var itens = API.getPlayersInRadiusOfPosition(10, blip.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
                API.sendChatMessageToPlayer(player, string.Format("Blip {0}", blip.ID));
        }

        foreach (var ped in peds)
        {
            var itens = API.getPlayersInRadiusOfPosition(10, ped.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
                API.sendChatMessageToPlayer(player, string.Format("PED {0}", ped.ID));
        }

        foreach (var atm in atms)
        {
            var itens = API.getPlayersInRadiusOfPosition(10, atm.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
                API.sendChatMessageToPlayer(player, string.Format("ATM {0}", atm.ID));
        }

        foreach (var ponto in pontos)
        {
            var itens = API.getPlayersInRadiusOfPosition(10, ponto.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
                API.sendChatMessageToPlayer(player, string.Format("Ponto {0}", ponto.ID));
        }

        foreach (var conce in concessionarias)
        {
            var itens = API.getPlayersInRadiusOfPosition(10, conce.Posicao).Where(p => p.name.Equals(player.name)).ToList();
            if (itens.Count > 0)
                API.sendChatMessageToPlayer(player, string.Format("Concessionária {0}", conce.ID));
        }
    }

    [Command("personagens")]
    public void CMD_Personagens(Client player, string usuario)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var param1 = new List<string>();
        var param2 = new List<string>();
        var str = string.Format("SELECT personagens.NomePersonagem FROM personagens INNER JOIN usuarios ON usuarios.ID=personagens.IDUsuario AND usuarios.NomeUsuario={0}", at(usuario));
        var cmd = new MySqlCommand(str, bancodados);
        var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            param1.Add(dr["NomePersonagem"].ToString().Replace("_", " "));
            param2.Add(string.Empty);
        }
        dr.Close();

        API.triggerClientEvent(player, "menusemresposta", "PERSONAGENS", "Personagem de " + usuario, 6, param1, param2);
    }

    [Command("usuario")]
    public void CMD_Usuario(Client player, string nomePersonagem)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var str = string.Format("SELECT usuarios.NomeUsuario FROM personagens INNER JOIN usuarios ON usuarios.ID=personagens.IDUsuario WHERE personagens.NomePersonagem={0}", at(nomePersonagem));
        var cmd = new MySqlCommand(str, bancodados);
        var dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            dr.Read();
            API.sendNotificationToPlayer(player, string.Format("O usuário de {0} é {1}.", nomePersonagem, dr["NomeUsuario"].ToString()));
            dr.Close();
        }
        else
        {
            API.sendNotificationToPlayer(player, nomePersonagem + " não foi localizado no banco de dados.");
            dr.Close();
        }
    }
    #endregion

    #region Game Admin 2 (3)
    [Command("irprop")]
    public void CMD_IrProp(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 3)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var prop = recuperarPropriedadePorID(id);
        if (prop == null)
        {
            EnviarMensagemErro(player, "Propriedade inválida.");
            return;
        }

        API.setEntityPosition(player, prop.EntradaFrente);
        EnviarMensagemSucesso(player, string.Format("Você foi até a propriedade {0}.", prop.ID));
    }

    [Command("irveh")]
    public void CMD_IrVeh(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var veh = recuperarVeiculoPorID(id);
        if (veh == null)
        {
            EnviarMensagemErro(player, "Veículo inválido.");
            return;
        }

        var pos = API.getEntityPosition(veh.Veh);
        pos.X = pos.X + 3;
        API.setEntityDimension(player, API.getEntityDimension(veh.Veh));
        API.setEntityPosition(player, pos);
        EnviarMensagemSucesso(player, string.Format("Você foi até o veículo {0}.", id));
    }

    [Command("trazerveh")]
    public void CMD_TrazerVeh(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 2)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var veh = recuperarVeiculoPorID(id);
        if (veh == null)
        {
            EnviarMensagemErro(player, "Veículo inválido.");
            return;
        }

        var pos = player.position;
        pos.X = pos.X + 3;
        API.setEntityDimension(veh.Veh, API.getEntityDimension(player));
        API.setEntityPosition(veh.Veh, pos);
        EnviarMensagemSucesso(player, string.Format("Você trouxe o veículo {0}.", id));
    }
    #endregion

    #region Game Admin 3 (4)

    #endregion

    #region Lead Admin (5)
    [Command("colete")]
    public void CMD_Colete(Client player, string idOrName, int colete)
    {
        if (API.getEntitySyncedData(player, "staff") < 5)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (colete < 0 || colete > 100)
        {
            EnviarMensagemErro(player, "Colete precisa ser entre 0 e 100.");
            return;
        }

        API.setPlayerArmor(target, colete);
        EnviarMensagemSucesso(player, string.Format("Você modificou o colete de {0} para {1}.", target.name.Replace("_", " "), colete));
        EnviarMensagemSucesso(target, string.Format("{0} modificou seu colete para {1}.", API.getEntitySyncedData(player, "usuario"), colete));
    }

    [Command("venderpropsinativas")]
    public void CMD_VenderPropsInativas(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 5)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var str = @"UPDATE propriedades 
            INNER JOIN personagens ON personagens.ID = propriedades.IDPersonagemProprietario
            SET IDPersonagemProprietario = 0
            WHERE DATEDIFF(now(), personagens.DataHoraUltimoAcesso) > 14";
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

        EnviarMensagemSucesso(player, "Você vendeu as propriedades inativas do servidor.");
    }
    #endregion

    #region Head Admin (6)
    [Command("criarfac", GreedyArg = true)]
    public void CMD_CriarFac(Client player, string nomeFaccao)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }
        if (faccoes.Count == MAX_FACCOES)
        {
            EnviarMensagemErro(player, "O limite de facções foi atingido.");
            return;
        }

        CriarFaccao(nomeFaccao);
        EnviarMensagemSucesso(player, "Faccção criada com sucesso!");
    }

    [Command("faccoes")]
    public void CMD_Faccoes(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (faccoes.Count == 0)
        {
            EnviarMensagemErro(player, "Nenhuma facção encontrada.");
            return;
        }

        var param1 = new List<string>();
        var param2 = new List<string>();
        foreach (var fac in faccoes)
        {
            param1.Add(fac.NomeFaccao);
            param2.Add(string.Format("ID: {0}", fac.ID));
        }

        API.triggerClientEvent(player, "menusemresposta", "FACÇÕES", "Listagem de Facções", 6, param1, param2);
    }

    [Command("editarfac", GreedyArg = true)]
    public void CMD_EditarFac(Client player, int id, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var faccao = recuperarFaccaoPorID(id);
        var index = faccoes.IndexOf(faccao);

        if (faccao == null)
        {
            EnviarMensagemErro(player, "Facção inválida.");
            return;
        }

        var strUpdate = string.Empty;
        switch (item)
        {
            case "nome":
                faccoes[index].NomeFaccao = valor;
                strUpdate = string.Format("UPDATE faccoes SET NomeFaccao = {0} WHERE ID = {1}", at(valor), id);
                break;
            case "abreviatura":
                faccoes[index].AbreviaturaFaccao = valor;
                strUpdate = string.Format("UPDATE faccoes SET AbreviaturaFaccao = {0} WHERE ID = {1}", at(valor), id);
                break;
            case "tipo":
                if (isInt(valor).Equals(0))
                {
                    EnviarMensagemErro(player, "Use apenas números para alterar o tipo da facção.");
                    return;
                }

                faccoes[index].TipoFaccao = (Faccao.Tipo)isInt(valor);
                strUpdate = string.Format("UPDATE faccoes SET TipoFaccao = {0} WHERE ID = {1}", valor, id);
                break;
            case "cor":
                if (valor.Length != 6)
                {
                    EnviarMensagemErro(player, "Cor inválida. A cor precisa ter 6 caracteres.");
                    return;
                }

                faccoes[index].CorFaccao = valor;
                strUpdate = string.Format("UPDATE faccoes SET CorFaccao = {0} WHERE ID = {1}", at(valor), id);
                break;
            case "rankgestor":
                if (isInt(valor) < 1 || isInt(valor) > MAX_FACCOES_RANKS)
                {
                    EnviarMensagemErro(player, string.Format("O rank precisa ser entre 1 e {0}.", MAX_FACCOES_RANKS));
                    return;
                }

                faccoes[index].IDRankGestor = isInt(valor);
                strUpdate = string.Format("UPDATE faccoes SET IDRankGestor = {0} WHERE ID = {1}", valor, id);
                break;
            case "ranklider":
                if (isInt(valor) < 1 || isInt(valor) > MAX_FACCOES_RANKS)
                {
                    EnviarMensagemErro(player, string.Format("O rank precisa ser entre 1 e {0}.", MAX_FACCOES_RANKS));
                    return;
                }

                faccoes[index].IDRankLider = isInt(valor);
                strUpdate = string.Format("UPDATE faccoes SET IDRankLider = {0} WHERE ID = {1}", valor, id);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Facção {0} editada com sucesso.", id));
    }

    [Command("lider")]
    public void CMD_Lider(Client player, string idOrName, int faccao)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        var fac = recuperarFaccaoPorID(faccao);

        if (fac == null)
        {
            EnviarMensagemErro(player, "Facção inválida.");
            return;
        }

        if (API.getEntitySyncedData(target, "idfaccao") != 0)
        {
            EnviarMensagemErro(player, "O jogador já está em uma facção.");
            return;
        }

        API.setEntitySyncedData(target, "idfaccao", faccao);
        API.setEntitySyncedData(target, "idrank", fac.IDRankLider);

        API.sendNotificationToPlayer(player, string.Format("~g~Você setou {0} como líder da facção {1}.", target.name.Replace("_", " "), faccao));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} setou você como líder da facção {1}.", API.getEntitySyncedData(player, "usuario"), faccao));
    }

    [Command("criarprop")]
    public void CMD_CriarProp(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (propriedades.Count >= MAX_PROPRIEDADES)
        {
            EnviarMensagemErro(player, "O limite de propriedades foi atingido.");
            return;
        }

        CriarPropriedade(player);
        EnviarMensagemSucesso(player, "Propriedade criada com sucesso.");
    }

    [Command("editarprop", GreedyArg = true)]
    public void CMD_EditarProp(Client player, int id = 0, string item = "", string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (item.Trim().Equals(string.Empty))
        {
            API.sendChatMessageToPlayer(player, "~r~[CMD] Editar Propriedade");
            API.sendChatMessageToPlayer(player, "Opções: endereco, tipo, faccao, prop, interior, level, valor, entradafrente.");
            return;
        }

        var prop = recuperarPropriedadePorID(id);
        if (prop == null)
        {
            EnviarMensagemErro(player, "Propriedade inválida.");
            return;
        }

        var strUpdate = string.Empty;
        switch (item)
        {
            case "endereco":
                strUpdate = string.Format("UPDATE propriedades SET Endereco = {0} WHERE ID = {1}", at(valor), id);
                break;
            case "tipo":
                strUpdate = string.Format("UPDATE propriedades SET TipoPropriedade = {0} WHERE ID = {1}", valor, id);
                break;
            case "faccao":
                strUpdate = string.Format("UPDATE propriedades SET IDFaccao={0} WHERE ID={1}", valor, id);
                break;
            case "prop":
                strUpdate = string.Format("UPDATE propriedades SET IDPropriedade = {0} WHERE ID = {1}", valor, id);
                break;
            case "interior":
                var pos = RecuperarPosicaoPorInterior(isInt(valor));
                strUpdate = string.Format("UPDATE propriedades SET Interior = {0}, SaidaFrentePosX = {1}, SaidaFrentePosZ = {2}, SaidaFrentePosY = {3} WHERE ID = {4}",
                    valor,
                    aFlt(pos.X),
                    aFlt(pos.Z),
                    aFlt(pos.Y),
                    id);
                break;
            case "level":
                strUpdate = string.Format("UPDATE propriedades SET Level = {0} WHERE ID = {1}", valor, id);
                break;
            case "valor":
                strUpdate = string.Format("UPDATE propriedades SET ValorPropriedade = {0} WHERE ID = {1}", valor, id);
                break;
            case "entradafrente":
                strUpdate = string.Format("UPDATE propriedades SET EntradaFrentePosX = {0}, EntradaFrentePosZ = {1}, EntradaFrentePosY = {2} WHERE ID = {3}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    id);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();

            RecarregarPropriedade(player, id, item, valor);
        }

        EnviarMensagemSucesso(player, string.Format("Propriedade {0} editada com sucesso.", id));
    }

    [Command("arma")]
    public void CMD_Arma(Client player, string idOrName, WeaponHash arma, int municao)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (API.getPlayerCurrentWeapon(target).ToString() != "Unarmed")
        {
            EnviarMensagemErro(player, "Este jogador já tem uma arma em mãos.");
            return;
        }

        API.givePlayerWeapon(target, arma, municao, false, true);
        EnviarMensagemSucesso(player, string.Format("Você deu para {0} a arma {1} com munição {2}.", target.name.Replace("_", " "),
            arma.ToString(), municao));

        CriarLog(LOG_HEADADMIN, string.Format("{0} /arma {1} {2} {3}",
            API.getEntitySyncedData(player, "usuario"),
            target.name.Replace("_", " "),
            arma.ToString(),
            municao
            ));
    }

    [Command("municao")]
    public void CMD_Municao(Client player, string idOrName, WeaponHash arma, int municao)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        API.setPlayerWeaponAmmo(target, arma, municao);

        CriarLog(LOG_HEADADMIN, string.Format("{0} /municao {1} {2} {3}",
            API.getEntitySyncedData(player, "usuario"),
            target.name.Replace("_", " "),
            arma.ToString(),
            municao
            ));
    }

    [Command("criarveh")]
    public void CMD_CriarVeh(Client player, VehicleHash modelo)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }
        CriarVeiculo(player, modelo, 0, 0, player.position, player.rotation, player.dimension);
        EnviarMensagemSucesso(player, "Veículo criado com sucesso.");
    }

    [Command("editarveh")]
    public void CMD_EditarVeh(Client player, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if(item == "")
        {
            API.sendChatMessageToPlayer(player, "[Opções]: Modelo, pos, faccao, gasolina");
            API.sendChatMessageToPlayer(player, "[Outros]: /editarvcor");
            return;
        }

        if (!player.isInVehicle)
        {
            EnviarMensagemErro(player, "Você não está em um veículo.");
            return;
        }

        var veh = recuperarVeiculo(player.vehicle);
        if (veh == null)
        {
            EnviarMensagemErro(player, "O veículo está bugado! Avise um desenvolvedor.");
            return;
        }

        var strUpdate = string.Empty;
        switch (item)
        {
            case "modelo":
                var myVehicle = API.vehicleNameToModel(valor);
                if (myVehicle == 0) return;

                var index = veiculos.IndexOf(veh);
                player.vehicle.delete();
                veiculos[index].Veh = API.createVehicle(myVehicle, player.position, player.rotation, 0, 0);
                API.setVehicleCustomPrimaryColor(veh.Veh, veh.Cor1.red, veh.Cor1.green, veh.Cor1.blue);
                API.setVehicleCustomSecondaryColor(veh.Veh, veh.Cor2.red, veh.Cor2.green, veh.Cor2.blue);
                API.setVehicleNumberPlate(veiculos[index].Veh, veiculos[index].Placa);
                API.setEntitySyncedData(veiculos[index].Veh, "id", veiculos[index].ID);
                API.setPlayerIntoVehicle(player, veiculos[veiculos.IndexOf(veh)].Veh, -1);

                strUpdate = string.Format("UPDATE veiculos SET ModeloVeiculo = {0} WHERE ID = {1}", at(myVehicle.ToString()), veh.ID);
                break;
            case "pos":
                strUpdate = string.Format("UPDATE veiculos SET PosX={0},PosZ={1},PosY={2},RotX={3},RotZ={4},RotY={5},Dimensao={6} WHERE ID={7}",
                    aFlt(player.vehicle.position.X),
                    aFlt(player.vehicle.position.Z),
                    aFlt(player.vehicle.position.Y),
                    aFlt(player.vehicle.rotation.X),
                    aFlt(player.vehicle.rotation.Z),
                    aFlt(player.vehicle.rotation.Y),
                    player.dimension,
                    veh.ID);
                break;
            case "faccao":
                var faccao = recuperarFaccaoPorID(isInt(valor));
                if (faccao == null)
                {
                    EnviarMensagemErro(player, "Facção inválida.");
                    return;
                }

                veiculos[veiculos.IndexOf(veh)].IDFaccao = faccao.ID;
                strUpdate = string.Format("UPDATE veiculos SET IDFaccao={0}, Spawnado=1 WHERE ID = {1}", valor, veh.ID);
                break;
            case "gasolina":
                if (isFloat(valor) > 100.0f) EnviarMensagemErro(player, "Máximo de gasolina: 100 L");
                if (isFloat(valor) < 0f) EnviarMensagemErro(player, "Mínimo de gasolina: 0 L");

                veiculos[veiculos.IndexOf(veh)].gasolina = isFloat(valor);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Veículo {0} editado com sucesso.", veh.ID));
    }

    [Command("editarvcor")]
    public void CMD_editarvcor(Client player, int cor, int r, int g, int b)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var veh = recuperarVeiculo(player.vehicle);
        if (veh == null)
        {
            EnviarMensagemErro(player, "O veículo está bugado! Avise um desenvolvedor.");
            return;
        }

        var strUpdate = string.Empty;
        switch (cor)
        {
            case 1:
                veh.Cor1.red = r;
                veh.Cor1.green = g;
                veh.Cor1.blue = b;
                API.setVehicleCustomPrimaryColor(veh.Veh, veh.Cor1.red, veh.Cor1.green, veh.Cor1.blue);
                strUpdate = string.Format("UPDATE veiculos SET Cor1R = {1}, Cor1G = {2}, Cor1B = {3} WHERE ID = {0}", veh.ID, r, g, b);
                break;
            case 2:
                API.setVehicleCustomSecondaryColor(veh.Veh, veh.Cor2.red, veh.Cor2.green, veh.Cor2.blue);
                strUpdate = string.Format("UPDATE veiculos SET Cor2R = {1}, Cor2G = {2}, Cor2B = {3} WHERE ID = {0}", veh.ID, r, g, b);
                break;
        }
        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Cor do veículo {0} editado com sucesso.", veh.ID));
    }

    [Command("ranks")]
    public void CMD_Ranks(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var faccao = recuperarFaccaoPorID(id);

        if (faccao == null)
        {
            EnviarMensagemErro(player, "Facção inválida.");
            return;
        }

        var param1 = new List<string>();
        var param2 = new List<string>();
        foreach (var rank in faccao.Ranks)
        {
            param1.Add(rank.NomeRank);
            param2.Add(string.Format("ID: {0} | Salário: ${1}", rank.ID, rank.Salario));
        }

        API.triggerClientEvent(player, "menusemresposta", faccao.AbreviaturaFaccao, "Ranks da Facção", 6, param1, param2);
    }

    [Command("editarrank", GreedyArg = true)]
    public void CMD_EditarRank(Client player, int faccao, int rank, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var fac = recuperarFaccaoPorID(faccao);
        if (fac == null)
        {
            EnviarMensagemErro(player, "Facção inválida.");
            return;
        }

        var index = faccoes.IndexOf(fac);

        if (rank < 1 || rank > MAX_FACCOES_RANKS)
        {
            EnviarMensagemErro(player, string.Format("O rank precisa ser entre 1 e {0}.", MAX_FACCOES_RANKS));
            return;
        }

        var rk = recuperarFaccaoRankPorID(fac.ID, rank);
        var findex = faccoes[index].Ranks.IndexOf(rk);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "nome":
                if (valor.Length > 25)
                {
                    EnviarMensagemErro(player, "A quantidade de caracteres do nome precisa ser no máximo 25.");
                    return;
                }

                faccoes[index].Ranks[findex].NomeRank = valor;
                strUpdate = string.Format("UPDATE ranks SET NomeRank = {0} WHERE ID = {1} AND IDFaccao = {2}", at(valor), rank, faccao);
                break;
            case "salario":
                if (isInt(valor).Equals(0))
                {
                    EnviarMensagemErro(player, "O salário precisa ser maior que 0.");
                    return;
                }

                faccoes[index].Ranks[findex].Salario = isInt(valor);
                strUpdate = string.Format("UPDATE ranks SET Salario = {0} WHERE ID = {1} AND IDFaccao = {2}", valor, rank, faccao);
                break;
            default: EnviarMensagemErro(player, "Opção inválida."); return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Rank {0} da facção {1} editado com sucesso.", rank, faccao));
    }

    [Command("criararmario")]
    public void CMD_CriarArmario(Client player, int faccao)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (faccao >= faccoes.Count)
        {
            EnviarMensagemErro(player, "Facção inválida.");
            return;
        }

        CriarArmario(player, faccao);
        EnviarMensagemSucesso(player, "Armário criado com sucesso.");
    }

    [Command("editararmario")]
    public void CMD_EditarArmario(Client player, int armario, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var index = armarios.IndexOf(arm);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "pos":
                strUpdate = string.Format("UPDATE armarios SET PosX = {0}, PosZ = {1}, PosY = {2}, Dimensao = {3} WHERE ID = {4}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    player.dimension,
                    arm.ID);

                arm.Posicao = player.position;
                arm.Dimensao = player.dimension;

                arm.Text.delete();

                arm.Text = API.createTextLabel("[/armario]", arm.Posicao, 30, (float)0.4, false, arm.Dimensao);
                break;
            case "faccao":
                var faccao = recuperarFaccaoPorID(isInt(valor));
                if (faccao == null)
                {
                    EnviarMensagemErro(player, "Facção inválida.");
                    return;
                }

                armarios[index].IDFaccao = faccao.ID;

                strUpdate = string.Format("UPDATE armarios SET IDFaccao = {0} WHERE ID = {1}", valor, arm.ID);
                break;
            case "rank":
                var rank = isInt(valor);
                if (rank < 1 || rank > MAX_FACCOES_RANKS)
                {
                    EnviarMensagemErro(player, "O rank precisa estar entre 1 e 20.");
                    return;
                }

                armarios[index].IDRank = rank;

                strUpdate = string.Format("UPDATE armarios SET IDRank = {0} WHERE ID = {1}", valor, arm.ID);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Armário {0} editado com sucesso.", arm.ID));
    }

    [Command("armarios")]
    public void CMD_Armarios(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (armarios.Count == 0)
        {
            EnviarMensagemErro(player, "Nenhum armário encotrado.");
            return;
        }

        var param1 = new List<string>();
        var param2 = new List<string>();
        foreach (var arm in armarios)
        {
            param1.Add(recuperarFaccaoPorID(arm.IDFaccao).NomeFaccao);
            param2.Add(string.Format("ID: {0}", arm.ID));
        }

        API.triggerClientEvent(player, "menusemresposta", "ARMÁRIOS", "Listagem de Armários", 6, param1, param2);
    }

    [Command("addaitem")]
    public void CMD_AddAItem(Client player, int armario, WeaponHash arma)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var item = recuperarArmarioItemPorArma(armario, arma.ToString());
        if (item != null)
        {
            EnviarMensagemErro(player, "Esse armário já possui esse item.");
            return;
        }

        var index = armarios.IndexOf(arm);

        var it = new Armario.Item();
        it.Arma = arma.ToString();
        armarios[index].Itens.Add(it);

        var strUpdate = string.Format("INSERT INTO armariositens (IDArmario, Arma) VALUES ({0}, {1})",
            arm.ID,
            at(arma.ToString()));
        var cmd = new MySqlCommand(strUpdate, bancodados);
        cmd.ExecuteNonQuery();

        EnviarMensagemSucesso(player, string.Format("Item adicionado no armário com sucesso.", arm.ID));
    }

    [Command("checararmario")]
    public void CMD_ChecarArmario(Client player, int armario)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var param1 = new List<string>();
        var param2 = new List<string>();
        foreach (var it in arm.Itens)
        {
            param1.Add(it.Arma);
            param2.Add(string.Format("Munição: {0} | Estoque: {1} | Rank: {2}", it.Municao, it.Estoque, it.IDRank));
        }

        API.triggerClientEvent(player, "menusemresposta", "ARMÁRIO " + arm.ID, "Listagem de Itens", 6, param1, param2);
    }

    [Command("irarmario")]
    public void CMD_IrArmario(Client player, int armario)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        API.setEntityPosition(player, arm.Posicao);
        API.setEntityDimension(player, arm.Dimensao);

        EnviarMensagemSucesso(player, string.Format("Você foi até o armário {0}.", arm.ID));
    }

    [Command("editaraitem")]
    public void CMD_EditarAItem(Client player, int armario, WeaponHash arma, string opcao, int valor)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var item = recuperarArmarioItemPorArma(armario, arma.ToString());
        if (item == null)
        {
            EnviarMensagemErro(player, "Arma inválida.");
            return;
        }

        var index = armarios.IndexOf(arm);
        var iitem = armarios[index].Itens.IndexOf(item);

        var strUpdate = string.Empty;
        switch (opcao)
        {
            case "estoque":
                armarios[index].Itens[iitem].Estoque = valor;

                strUpdate = string.Format("UPDATE armariositens SET Estoque = {0} WHERE IDArmario = {1} AND Arma = {2}", valor, arm.ID, at(arma.ToString()));
                break;
            case "municao":
                armarios[index].Itens[iitem].Municao = valor;

                strUpdate = string.Format("UPDATE armariositens SET Municao = {0} WHERE IDArmario = {1} AND Arma = {2}", valor, arm.ID, at(arma.ToString()));
                break;
            case "pintura":
                armarios[index].Itens[iitem].Pintura = valor;

                strUpdate = string.Format("UPDATE armariositens SET Pintura = {0} WHERE IDArmario = {1} AND Arma = {2}", valor, arm.ID, at(arma.ToString()));
                break;
            case "rank":
                if (valor < 1 || valor > MAX_FACCOES_RANKS)
                {
                    EnviarMensagemErro(player, "O rank precisa estar entre 1 e 20.");
                    return;
                }

                armarios[index].Itens[iitem].IDRank = valor;

                strUpdate = string.Format("UPDATE armariositens SET IDRank = {0} WHERE IDArmario = {1} AND Arma = {2}", valor, arm.ID, at(arma.ToString()));
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("{0} editado com sucesso.", arma.ToString()));
    }

    [Command("excluiraitem")]
    public void CMD_ExcluirAItem(Client player, int armario, WeaponHash arma)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var item = recuperarArmarioItemPorArma(armario, arma.ToString());
        if (item == null)
        {
            EnviarMensagemErro(player, "Arma inválida.");
            return;
        }

        var index = armarios.IndexOf(arm);

        var str = string.Format("DELETE FROM armariositens WHERE IDArmario = {0} AND Arma = {1}", arm.ID, at(arma.ToString()));
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        armarios[index].Itens.Remove(item);

        EnviarMensagemSucesso(player, string.Format("{0} excluído com sucesso.", arma.ToString()));
    }

    [Command("excluirarmario")]
    public void CMD_ExcluirArmario(Client player, int armario)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        arm.Text.delete();
        var str = string.Format("DELETE FROM armarios WHERE ID = {0}; DELETE FROM armariositens WHERE IDArmario = {0};", armario);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        armarios.Remove(arm);

        EnviarMensagemSucesso(player, string.Format("Armário {0} excluído com sucesso.", armario));
    }

    [Command("addaitemcomp")]
    public void CMD_AddAItemComp(Client player, int armario, WeaponHash arma, WeaponComponent comp)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var item = recuperarArmarioItemPorArma(armario, arma.ToString());
        if (item == null)
        {
            EnviarMensagemErro(player, "Arma inválida.");
            return;
        }

        var index = armarios.IndexOf(arm);
        var indexitem = armarios[index].Itens.IndexOf(item);

        var componentes = new List<WeaponComponent>();

        if (!armarios[index].Itens[indexitem].Componentes.Equals(string.Empty))
            componentes = JsonConvert.DeserializeObject<List<WeaponComponent>>(armarios[index].Itens[indexitem].Componentes);

        if (componentes.Contains(comp))
        {
            EnviarMensagemErro(player, "A arma já possui esse componente.");
            return;
        }
        componentes.Add(comp);

        armarios[index].Itens[indexitem].Componentes = JsonConvert.SerializeObject(componentes);

        var strUpdate = string.Format("UPDATE armariositens SET Componentes={0} WHERE IDArmario={1} AND Arma={2}",
            at(armarios[index].Itens[indexitem].Componentes),
            arm.ID,
            at(arma.ToString()));
        var cmd = new MySqlCommand(strUpdate, bancodados);
        cmd.ExecuteNonQuery();

        EnviarMensagemSucesso(player, string.Format("{0} adicionado na arma com sucesso.", comp.ToString()));
    }

    [Command("excluiraitemcomps")]
    public void CMD_ExcluirAItemComps(Client player, int armario, WeaponHash arma)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var arm = recuperarArmarioPorID(armario);
        if (arm == null)
        {
            EnviarMensagemErro(player, "Armário inválido.");
            return;
        }

        var item = recuperarArmarioItemPorArma(armario, arma.ToString());
        if (item == null)
        {
            EnviarMensagemErro(player, "Arma inválida.");
            return;
        }

        var index = armarios.IndexOf(arm);
        var indexitem = armarios[index].Itens.IndexOf(item);

        armarios[index].Itens[indexitem].Componentes = string.Empty;

        var strUpdate = string.Format("UPDATE armariositens SET Componentes='' WHERE IDArmario={0} AND Arma={1}",
            arm.ID,
            at(arma.ToString()));
        var cmd = new MySqlCommand(strUpdate, bancodados);
        cmd.ExecuteNonQuery();

        EnviarMensagemSucesso(player, string.Format("Componentes de {0} excluídos com sucesso.", arma.ToString()));
    }

    [Command("excluirprop")]
    public void CMD_ExcluirProp(Client player, int prop)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var p = recuperarPropriedadePorID(prop);
        if (p == null)
        {
            EnviarMensagemErro(player, "Propriedade inválida.");
            return;
        }

        if (p.TextFrente != null) p.TextFrente.delete();
        if (p.TextValorFrente != null) p.TextValorFrente.delete();
        if (p.MarkerFrente != null) p.MarkerFrente.delete();

        var str = string.Format("DELETE FROM propriedades WHERE ID = {0}", prop);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        propriedades.Remove(p);

        EnviarMensagemSucesso(player, string.Format("Propriedade {0} excluída com sucesso.", prop));
    }

    [Command("irint")]
    public void CMD_IrInt(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 6)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (API.getEntitySyncedData(player, "ipl") != "")
            API.sendNativeToPlayer(player, Hash.REMOVE_IPL, API.getEntitySyncedData(player, "ipl"));

        API.setEntitySyncedData(player, "ipl", string.Empty);
        var ipl = RecuperarIPLPorInterior(id);
        if (ipl != "")
        {
            API.sendNativeToPlayer(player, Hash.REQUEST_IPL, ipl);
            API.setEntitySyncedData(player, "ipl", ipl);
        }

        API.setEntityPosition(player, RecuperarPosicaoPorInterior(id));
        API.setEntityDimension(player, 0);

        EnviarMensagemSucesso(player, string.Format("Você foi para o interior {0}.", id));
    }
    #endregion

    #region Developer (7)

    [Command("getrot")]
    public void CMD_GetRot(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 7)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.sendNotificationToPlayer(player, string.Format("X:{0} Y:{1} Z:{2}",
            aFlt(player.rotation.X),
            aFlt(player.rotation.Y),
            aFlt(player.rotation.Z)));
    }

    [Command("getpos")]
    public void CMD_GetPos(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 7)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.sendNotificationToPlayer(player, string.Format("X:{0} Y:{1} Z:{2}",
            aFlt(player.position.X),
            aFlt(player.position.Y),
            aFlt(player.position.Z)));
    }

    [Command("irpos")]
    public void CMD_IrPos(Client player, float x, float y, float z)
    {
        if (API.getEntitySyncedData(player, "staff") < 7)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.setEntityPosition(player, new Vector3(x, y, z));
        API.sendNotificationToPlayer(player, string.Format("Você foi para X:{0} Y:{1} Z:{2}",
            aFlt(player.position.X),
            aFlt(player.position.Y),
            aFlt(player.position.Z)));
    }

    [Command("gmx")]
    public void CMD_GMX(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 7)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        foreach (Client target in API.getAllPlayers())
            SalvarPersonagem(target);

        API.sendNotificationToAll("~r~O servidor será reiniciado!");

        foreach (Client target in API.getAllPlayers())
            API.kickPlayer(player);
    }

    [Command("fix")]
    public void CMD_Fix(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 7)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (!player.isInVehicle)
        {
            EnviarMensagemErro(player, "Você precisa estar em um veículo.");
            return;
        }

        API.repairVehicle(player.vehicle);
        EnviarMensagemSucesso(player, "Você reparou seu veículo.");
    }
    #endregion

    #region Management (8)
    [Command("staff")]
    public void CMD_Staff(Client player, string idOrName, int staff)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        if (staff < 0 || staff > 8)
        {
            EnviarMensagemErro(player, "Staff precisa ser entre 0 e 8.");
            return;
        }

        API.setEntitySyncedData(target, "staff", staff);
        API.sendNotificationToPlayer(player, string.Format("~g~Você setou {0} como staff {1} ({2}).", API.getEntitySyncedData(target, "usuario"), recuperarNomeStaff(staff), staff));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} setou você como staff {1} ({2}).", API.getEntitySyncedData(player, "usuario"), recuperarNomeStaff(staff), staff));
    }

    [Command("tempo")]
    public void CMD_Tempo(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        if (id < 1 || id > 12)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 12.");
            return;
        }

        API.setWeather(id);
        API.sendNotificationToPlayer(player, string.Format("~g~Você mudou o tempo para {0}.", id));
    }

    [Command("dinheiro")]
    public void CMD_Dinheiro(Client player, string idOrName, int dinheiro)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, idOrName);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        API.setEntitySyncedData(target, "dinheiro", dinheiro);
        API.sendNotificationToPlayer(player, string.Format("~g~Você alterou o dinheiro de {0} para {1}.", target.name.Replace("_", " "), dinheiro));
        API.sendNotificationToPlayer(target, string.Format("~g~{0} alterou seu dinheiro para {1}.", API.getEntitySyncedData(player, "usuario"), dinheiro));
    }

    [Command("criarblip")]
    public void CMD_CriarBlip(Client player, int tipo, int cor)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        CriarBlip(player, tipo, cor);
        API.sendNotificationToPlayer(player, "~g~Blip criado com sucesso!");
    }

    [Command("criaratm")]
    public void CMD_CriarATM(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        CriarATM(player);
        EnviarMensagemSucesso(player, "ATM criado com sucesso.");
    }

    [Command("criarped")]
    public void CMD_CriarPED(Client player, PedHash skin)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        CriarPED(player, skin);
        EnviarMensagemSucesso(player, "PED criado com sucesso.");
    }

    [Command("excluirblip")]
    public void CMD_ExcluirBlip(Client player, int blip)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var bp = recuperarBlipPorID(blip);
        if (bp == null)
        {
            EnviarMensagemErro(player, "Blip inválido.");
            return;
        }

        bp.Blip.delete();
        var str = string.Format("DELETE FROM blips WHERE ID = {0}", blip);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        blips.Remove(bp);

        EnviarMensagemSucesso(player, string.Format("Blip {0} excluído com sucesso.", blip));
    }

    [Command("editarblip")]
    public void CMD_EditarBlip(Client player, int blip, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var bp = recuperarBlipPorID(blip);
        if (bp == null)
        {
            EnviarMensagemErro(player, "Blip inválido.");
            return;
        }

        var index = blips.IndexOf(bp);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "pos":
                strUpdate = string.Format("UPDATE blips SET PosX = {0}, PosZ = {1}, PosY = {2} WHERE ID = {3}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    bp.ID);

                blips[index].Blip.delete();

                blips[index].Posicao = player.position;
                blips[index].Blip = API.createBlip(blips[index].Posicao, (float)2);
                API.setBlipSprite(blips[index].Blip, blips[index].Tipo);
                API.setBlipColor(blips[index].Blip, blips[index].Cor);
                break;
            case "tipo":
                blips[index].Tipo = isInt(valor);
                API.setBlipSprite(blips[index].Blip, blips[index].Tipo);

                strUpdate = string.Format("UPDATE blips SET Tipo = {0} WHERE ID = {1}", valor, bp.ID);
                break;
            case "cor":
                blips[index].Cor = isInt(valor);
                API.setBlipColor(blips[index].Blip, blips[index].Cor);

                strUpdate = string.Format("UPDATE blips SET Cor = {0} WHERE ID = {1}", valor, bp.ID);
                break;
            case "nome":
                blips[index].Nome = valor;

                strUpdate = string.Format("UPDATE blips SET Nome = {0} WHERE ID = {1}", at(valor), bp.ID);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Blip {0} editado com sucesso.", bp.ID));
    }

    [Command("irblip")]
    public void CMD_IrBlip(Client player, int blip)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var bp = recuperarBlipPorID(blip);
        if (bp == null)
        {
            EnviarMensagemErro(player, "Blip inválido.");
            return;
        }

        API.setEntityPosition(player, bp.Posicao);
        EnviarMensagemSucesso(player, string.Format("Você foi até o blip {0}.", bp.ID));
    }

    [Command("excluirped")]
    public void CMD_ExcluirPED(Client player, int ped)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pd = recuperarPedPorID(ped);
        if (pd == null)
        {
            EnviarMensagemErro(player, "PED inválido.");
            return;
        }

        pd.Ped.delete();
        var str = string.Format("DELETE FROM peds WHERE ID = {0}", ped);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        peds.Remove(pd);

        EnviarMensagemSucesso(player, string.Format("PED {0} excluído com sucesso.", ped));
    }

    [Command("irped")]
    public void CMD_IrPed(Client player, int ped)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pd = recuperarPedPorID(ped);
        if (pd == null)
        {
            EnviarMensagemErro(player, "PED inválido.");
            return;
        }

        API.setEntityPosition(player, pd.Posicao);
        API.setEntityDimension(player, pd.Dimensao);
        EnviarMensagemSucesso(player, string.Format("Você foi até o PED {0}.", pd.ID));
    }

    [Command("editarped")]
    public void CMD_EditarPED(Client player, int ped, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pd = recuperarPedPorID(ped);
        if (pd == null)
        {
            EnviarMensagemErro(player, "PED inválido.");
            return;
        }

        var index = peds.IndexOf(pd);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "pos":
                strUpdate = string.Format("UPDATE peds SET PosX = {0}, PosZ = {1}, PosY = {2}, Dimensao = {3} WHERE ID = {4}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    player.dimension,
                    pd.ID);

                peds[index].Ped.delete();

                peds[index].Posicao = player.position;
                peds[index].Dimensao = player.dimension;
                peds[index].Ped = API.createPed(peds[index].Skin, peds[index].Posicao, peds[index].Rotacao, peds[index].Dimensao);
                break;
            case "rot":
                peds[index].Ped.delete();

                peds[index].Rotacao = isInt(valor);
                peds[index].Ped = API.createPed(peds[index].Skin, peds[index].Posicao, peds[index].Rotacao, peds[index].Dimensao);

                strUpdate = string.Format("UPDATE peds SET Rotacao = {0} WHERE ID = {1}", isInt(valor), pd.ID);
                break;
            case "modelo":
                peds[index].Skin = (PedHash)isLong(valor);
                peds[index].Ped.delete();
                peds[index].Ped = API.createPed(peds[index].Skin, peds[index].Posicao, peds[index].Rotacao, peds[index].Dimensao);

                strUpdate = string.Format("UPDATE peds SET Skin = {0} WHERE ID = {1}", at(valor), pd.ID);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("PED {0} editado com sucesso.", pd.ID));
    }

    [Command("excluiratm")]
    public void CMD_ExcluirATM(Client player, int atm)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var at = recuperarATMPorId(atm);
        if (at == null)
        {
            EnviarMensagemErro(player, "ATM inválido.");
            return;
        }

        at.Text.delete();
        var str = string.Format("DELETE FROM atms WHERE ID = {0}", atm);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        atms.Remove(at);

        EnviarMensagemSucesso(player, string.Format("ATM {0} excluída com sucesso.", atm));
    }

    [Command("editaratm")]
    public void CMD_EditarATM(Client player, int atm, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var at = recuperarATMPorId(atm);
        if (at == null)
        {
            EnviarMensagemErro(player, "ATM inválido.");
            return;
        }

        var index = atms.IndexOf(at);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "pos":
                strUpdate = string.Format("UPDATE atms SET PosX={0}, PosZ ={1}, PosY ={2} WHERE ID = {3}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    at.ID);

                atms[index].Text.delete();

                atms[index].Posicao = player.position;
                atms[index].Text = API.createTextLabel("ATM", atms[index].Posicao, 30, (float)0.4);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("ATM {0} editada com sucesso.", at.ID));
    }

    [Command("iratm")]
    public void CMD_IrATM(Client player, int atm)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var at = recuperarATMPorId(atm);
        if (at == null)
        {
            EnviarMensagemErro(player, "ATM inválido.");
            return;
        }

        API.setEntityPosition(player, at.Posicao);
        EnviarMensagemSucesso(player, string.Format("Você foi até a ATM {0}.", at.ID));
    }

    [Command("excluirveh")]
    public void CMD_ExcluirVeh(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var veh = recuperarVeiculoPorID(id);
        if (veh == null)
        {
            EnviarMensagemErro(player, "Apenas veículos spawnados podem ser excluídos.");
            return;
        }

        if (veh.IDPersonagemProprietario != 0)
        {
            EnviarMensagemErro(player, "Apenas veículos sem proprietário podem ser excluídos.");
            return;
        }

        veh.Veh.delete();
        veiculos.Remove(veh);

        var str = string.Format("DELETE FROM veiculos WHERE ID = {0}", id);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

        EnviarMensagemSucesso(player, string.Format("Veículo {0} excluído com sucesso.", id));
    }

    [Command("specall")]
    public void CMD_SpecAll(Client player)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        API.setPlayerToSpectator(player);
    }

    [Command("criarponto")]
    public void CMD_CriarPonto(Client player, int tipo)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        CriarPonto(player, tipo);
        EnviarMensagemSucesso(player, "Ponto criado com sucesso.");
    }

    [Command("excluirponto")]
    public void CMD_ExcluirPonto(Client player, int ponto)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pt = recuperarPontoPorID(ponto);
        if (pt == null)
        {
            EnviarMensagemErro(player, "Ponto inválido.");
            return;
        }

        pt.Text.delete();
        var str = string.Format("DELETE FROM pontos WHERE ID = {0}", ponto);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        pontos.Remove(pt);

        EnviarMensagemSucesso(player, string.Format("Ponto {0} excluído com sucesso.", ponto));
    }

    [Command("irponto")]
    public void CMD_IrPonto(Client player, int ponto)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pt = recuperarPontoPorID(ponto);
        if (pt == null)
        {
            EnviarMensagemErro(player, "Ponto inválido.");
            return;
        }

        API.setEntityPosition(player, pt.Posicao);
        EnviarMensagemSucesso(player, string.Format("Você foi até o ponto {0}.", pt.ID));
    }

    [Command("editarponto")]
    public void CMD_EditarPonto(Client player, int ponto, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var pt = recuperarPontoPorID(ponto);
        if (pt == null)
        {
            EnviarMensagemErro(player, "Ponto inválido.");
            return;
        }

        var index = pontos.IndexOf(pt);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "pos":
                strUpdate = string.Format("UPDATE pontos SET PosX = {0}, PosZ = {1}, PosY = {2} WHERE ID = {3}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    pt.ID);

                pontos[index].Text.delete();

                pontos[index].Posicao = player.position;
                pontos[index].Text = API.createTextLabel(pontos[index].Descricao, pontos[index].Posicao, 30, (float)0.4);
                break;
            case "tipo":
                pontos[index].TipoPonto = (Ponto.Tipo)isInt(valor);
                pontos[index].Descricao = recuperarDescricaoPorTipo(isInt(valor));

                strUpdate = string.Format("UPDATE pontos SET Tipo = {0} WHERE ID = {1}", valor, pt.ID);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Ponto {0} editado com sucesso.", pt.ID));
    }

    [Command("criarconce")]
    public void CMD_CriarConce(Client player, string tipo)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        CriarConce(player, tipo);
        EnviarMensagemSucesso(player, "Concessionária criada com sucesso.");
    }

    [Command("excluirconce")]
    public void CMD_ExcluirConce(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var conce = recuperarConcePorID(id);
        if (conce == null)
        {
            EnviarMensagemErro(player, "Concessionária inválida.");
            return;
        }

        conce.Text.delete();
        var str = string.Format("DELETE FROM concessionarias WHERE ID = {0}", id);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        concessionarias.Remove(conce);

        EnviarMensagemSucesso(player, string.Format("Concessionária {0} excluída com sucesso.", id));
    }

    [Command("irconce")]
    public void CMD_IrConce(Client player, int id)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var conce = recuperarConcePorID(id);
        if (conce == null)
        {
            EnviarMensagemErro(player, "Concessionária inválida.");
            return;
        }

        API.setEntityPosition(player, conce.Posicao);
        EnviarMensagemSucesso(player, string.Format("Você foi até a concessionária {0}.", conce.ID));
    }

    [Command("editarconce")]
    public void CMD_EditarConce(Client player, int id, string item, string valor = "")
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var conce = recuperarConcePorID(id);
        if (conce == null)
        {
            EnviarMensagemErro(player, "Concessionária inválida.");
            return;
        }

        var index = concessionarias.IndexOf(conce);

        var strUpdate = string.Empty;
        switch (item)
        {
            case "pos":
                strUpdate = string.Format("UPDATE concessionarias SET PosX = {0}, PosZ = {1}, PosY = {2} WHERE ID = {3}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    conce.ID);

                concessionarias[index].Text.delete();

                concessionarias[index].Posicao = player.position;
                concessionarias[index].Text = API.createTextLabel("[/v comprar]", concessionarias[index].Posicao, 30, (float)0.4);
                break;
            case "vpos":
                strUpdate = string.Format("UPDATE concessionarias SET PosX_v = {0}, PosZ_v = {1}, PosY_v = {2} WHERE ID = {3}",
                    aFlt(player.position.X),
                    aFlt(player.position.Z),
                    aFlt(player.position.Y),
                    conce.ID);

                concessionarias[index].Posicao_vspawn = player.position;
                break;
            case "tipo":
                concessionarias[index].Tipo = valor;

                strUpdate = string.Format("UPDATE concessionarias SET Tipo = {0} WHERE ID = {1}", at(valor), conce.ID);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                return;
        }

        if (!strUpdate.Equals(string.Empty))
        {
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();
        }

        EnviarMensagemSucesso(player, string.Format("Concessionária {0} editada com sucesso.", conce.ID));
    }

    [Command("editarconceveh")]
    public void CMD_EditarConceVeh(Client player, VehicleHash veiculo, int preco)
    {
        if (API.getEntitySyncedData(player, "staff") < 8)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var veh = recuperarVeiculoConcePorNome(veiculo.ToString());
        if (veh == null)
        {
            EnviarMensagemErro(player, "Veículo inválido.");
            return;
        }

        var index = veiculosConce.IndexOf(veh);

        var strUpdate = string.Format("UPDATE concessionariasveiculos SET Preco = {0} WHERE Nome = {1}", preco, at(veh.Nome));
        var cmd = new MySqlCommand(strUpdate, bancodados);
        cmd.ExecuteNonQuery();

        veiculosConce[index].Preco = preco;

        EnviarMensagemSucesso(player, string.Format("Veículo {0} editado com sucesso.", veiculo.ToString()));
    }
    #endregion

    #endregion

    #region Comandos Anims

    [Command("animlist")]
    public void CMD_Animlist(Client player, int pagina = 1)
    {
        switch (pagina)
        {
            case 1:
                API.sendChatMessageToPlayer(player, "/sa /stopanim /handsup /smoke /lean /crossarms /pushups ");
                API.sendChatMessageToPlayer(player, "/situps /blunt /afishing /acop / idle /barra /kneel");
                API.sendChatMessageToPlayer(player, "/revistarc /ajoelhar /drink /morto /gsign /hurry /cair");
                API.sendChatMessageToPlayer(player, "[ANIMS]: Para ver a próxima página use: /animlist 2");
                break;
            case 2:
                API.sendChatMessageToPlayer(player, "/jogado /reparando /luto /bar /necessidades /meth /mijar");
                API.sendChatMessageToPlayer(player, "/wsup /render /mirar /sentar /dormir /pixar /sexo");
                API.sendChatMessageToPlayer(player, "/incar /police");
                API.sendChatMessageToPlayer(player, "[ANIMS]: Para ver a página anterior use: /animlist 1");
                break;
        }
    }

    [Command("sa")]
    public void CMD_Stopanim1(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (API.getEntitySyncedData(player.handle, "NaoPodePararAnim") == 1)
            return;

        API.stopPlayerAnimation(player);
    }
    [Command("stopanim")]
    public void CMD_Stopanim(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (API.getEntitySyncedData(player.handle, "NaoPodePararAnim") == 1)
            return;

        API.stopPlayerAnimation(player);
    }

    [Command("handsup", "~y~USO: ~w~/handsup [1-2]")]
    public void CMD_Handsup(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missminuteman_1ig_2", "handsup_base"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@mp_player_intincarsurrenderstd@ds@", "idle_a"); break;
        }
    }

    [Command("smoke", "~y~USO: ~w~/smoke [1-3]")]
    public void CMD_Smoke(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking@male@male_a@enter", "enter"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking@male@male_a@base", "base"); break;
            case 3: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking@male@male_a@exit", "exit"); break;
        }
    }

    [Command("lean", "~y~USO: ~w~/lean [1-7]")]
    public void CMD_Lean(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 7)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 7.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_leaning@male@wall@back@hands_together@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_leaning@male@wall@back@foot_up@base", "base"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_leaning@male@wall@back@legs_crossed@base", "base"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "misscarstealfinale", "packer_idle_base_trevor"); break;
            case 5: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "switch@michael@marina", "loop"); break;
            case 6: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "switch@michael@pier", "pier_lean_smoke_idle"); break;
            case 7: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "switch@michael@sitting_on_car_premiere", "sitting_on_car_premiere_loop_player"); break;
        }
    }

    [Command("police", "~y~USO: ~w~/police [1-6]")]
    public void CMD_police(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 6)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 6.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@code_human_police_crowd_control@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@code_human_police_crowd_control@idle_a", "idle_a"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@code_human_police_crowd_control@idle_b", "idle_d"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@code_human_police_investigate@base", "base"); break;
            case 5: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@code_human_police_investigate@idle_a", "idle_a"); break;
            case 6: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@code_human_police_investigate@idle_b", "idle_f"); break;
        }
    }

    [Command("incar", "~y~USO: ~w~/incar [1-3]")]
    public void CMD_incar(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@incar@male@patrol@ds@idle_b", "idle_d"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@incar@male@patrol@ds@base", "base"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@incar@male@patrol@ps@idle_a", "idle_a"); break;
        }
    }

    [Command("crossarms", "~y~USO: ~w~/crossarms [1-2]")]
    public void CMD_Crossarms(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_hang_out_street@male_c@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missheistdockssetup1ig_10@base", "talk_pipe_base_worker2"); break;
        }
    }

    [Command("pushups", "~y~USO: ~w~/pushups [1-4]")]
    public void CMD_Pushups(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 4)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 4.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@world_human_push_ups@male@enter", "enter"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@world_human_push_ups@male@idle_a", "idle_a"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_push_ups@male@base", "base"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@world_human_push_ups@male@exit", "exit"); break;
        }
    }

    [Command("situps", "~y~USO: ~w~/situps [1-4]")]
    public void CMD_Situps(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 4)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 4.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@world_human_sit_ups@male@enter", "enter"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_sit_ups@male@idle_a", "idle_a"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_sit_ups@male@base", "base"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@world_human_sit_ups@male@exit", "exit"); break;
        }
    }

    [Command("blunt", "~y~USO: ~w~/blunt [1-2]")]
    public void CMD_Blunt(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking_pot@male@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_smoking_pot@male@idle_a", "idle_a"); break;
        }
    }

    [Command("afishing", "~y~USO: ~w~/afishing [1-3]")]
    public void CMD_AFishing(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_stand_fishing@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_stand_fishing@idle_a", "idle_a"); break;
            case 3: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_stand_fishing@idle_a", "idle_c"); break;
        }
    }

    [Command("acop")]
    public void CMD_ACop(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_cop_idles@male@base", "base");
    }

    [Command("idle", "~y~USO: ~w~/idle [1-3]")]
    public void CMD_Idle(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_drug_dealer_hard@male@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_drug_dealer_hard@male@idle_a", "idle_a"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@world_human_drug_dealer_hard@male@idle_b", "idle_d"); break;
        }
    }

    [Command("barra", "~y~USO: ~w~/barra [1-3]")]
    public void CMD_Barra(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@prop_human_muscle_chin_ups@male@enter", "enter"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@prop_human_muscle_chin_ups@male@base", "base"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@prop_human_muscle_chin_ups@male@exit", "exit_flee"); break;
        }
    }

    [Command("kneel")]
    public void CMD_Kneel(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@medic@standing@tendtodead@base", "base");
    }

    [Command("revistarc")]
    public void CMD_RevistarC(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "amb@medic@standing@tendtodead@idle_a", "idle_a");
    }

    [Command("ajoelhar", "~y~USO: ~w~/ajoelhar [1-4]")]
    public void CMD_Ajoelhar(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 4)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 4.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@medic@standing@kneel@enter", "enter"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@medic@standing@kneel@base", "base"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@medic@standing@kneel@idle_a", "idle_a"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "amb@medic@standing@kneel@exit", "exit_flee"); break;
        }
    }

    [Command("drink", "~y~USO: ~w~/drink [1-3]")]
    public void CMD_Drink(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_drinking@beer@male@base", "base"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_drinking@coffee@male@base", "base"); break;
            case 3: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@world_human_drinking@coffee@female@base", "base"); break;
        }
    }

    [Command("morto", "~y~USO: ~w~/morto [1-2]")]
    public void CMD_Morto(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "missfinale_c1@", "lying_dead_player0"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "misslamar1dead_body", "dead_idle"); break;
        }
    }

    [Command("gsign", "~y~USO: ~w~/gsign [1-2]")]
    public void CMD_Gsign(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_player_int_uppergang_sign_a", "mp_player_int_gang_sign_a"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_player_int_uppergang_sign_b", "mp_player_int_gang_sign_b"); break;
        }
    }

    [Command("hurry", "~y~USO: ~w~/hurry [1-2]")]
    public void CMD_Hurry(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missheist_agency3aig_18", "say_hurry_up_a"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missheist_agency3aig_18", "say_hurry_up_b"); break;
        }
    }

    [Command("cair", "~y~USO: ~w~/cair [1-2]")]
    public void CMD_Cair(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "mp_bank_heist_1", "prone_l_loop"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "mp_bank_heist_1", "prone_r_loop"); break;
        }
    }

    [Command("wsup", "~y~USO: ~w~/wsup [1-2]")]
    public void CMD_Wsup(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "rcmme_amanda1", "pst_arrest_loop_owner"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missheist_agency2aig_12", "look_at_plan_b"); break;
        }
    }

    [Command("render", "~y~USO: ~w~/render [1-2]")]
    public void CMD_Render(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "random@arrests@busted", "idle_c"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "random@arrests", "kneeling_arrest_idle"); break;
        }
    }

    [Command("mirar", "~y~USO: ~w~/mirar [1-2]")]
    public void CMD_Mirar(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "combat@aim_variations@arrest", "cop_med_arrest_01"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "missfbi2", "franklin_sniper_crouch"); break;
        }
    }

    [Command("sentar", "~y~USO: ~w~/sentar [1-8]")]
    public void CMD_Sentar(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 8)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 8.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@sitting", "idle"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_mic"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@on_sofa", "base_michael"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "safe@franklin@ig_13", "base"); break;
            case 5: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@bench", "bench_on_phone_idle"); break;
            case 6: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@parkbench_smoke_ranger", "parkbench_smoke_ranger_loop"); break;
            case 7: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@smoking2", "loop"); break;
            case 8: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_jmy"); break;
        }
    }

    [Command("dormir", "~y~USO: ~w~/dormir [1-2]")]
    public void CMD_Dormir(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@franklin@napping", "002333_01_fras_v2_10_napping_idle"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@dumpster", "002002_01_trvs_14_dumpster_idle"); break;
        }
    }

    [Command("pixar", "~y~USO: ~w~/pixar [1-2]")]
    public void CMD_Pixar(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "switch@franklin@lamar_tagging_wall", "lamar_tagging_wall_loop_lamar"); break;
            case 2: API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "switch@franklin@lamar_tagging_wall", "lamar_tagging_exit_loop_lamar"); break;
        }
    }

    [Command("sexo", "~y~USO: ~w~/sexo [1-5]")]
    public void CMD_Sexo(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 5)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 5.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "switch@trevor@garbage_food", "loop_trevor"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "switch@trevor@head_in_sink", "trev_sink_idle"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper"); break;
            case 4: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "misscarsteal2pimpsex", "shagloop_hooker"); break;
            case 5: API.playPlayerAnimation(player, (int)AnimationFlags.Loop, "misscarsteal2pimpsex", "shagloop_pimp"); break;
        }
    }

    [Command("jogado", "~y~USO: ~w~/jogado [1-3]")]
    public void CMD_Jogado(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 3)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 3.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@slouched_get_up", "trev_slouched_get_up_idle"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@naked_island", "loop"); break;
            case 3: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "rcm_barry3", "barry_3_sit_loop"); break;
        }
    }

    [Command("reparando", "~y~USO: ~w~/reparando [1-2]")]
    public void CMD_Reparando(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@garbage_food", "loop_trevor"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@puking_into_fountain", "trev_fountain_puke_loop"); break;
        }
    }

    [Command("luto", "~y~USO: ~w~/luto [1-2]")]
    public void CMD_Luto(Client player, int id)
    {
        if (!ChecarPlayerAnim(player))
            return;

        if (id < 1 || id > 2)
        {
            EnviarMensagemErro(player, "ID precisa ser entre 1 e 2.");
            return;
        }

        switch (id)
        {
            case 1: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@rejected_entry", "001396_01_mics3_6_rejected_entry_idle_bouncer"); break;
            case 2: API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@michael@talks_to_guard", "001393_02_mics3_3_talks_to_guard_idle_guard"); break;
        }
    }

    [Command("bar")]
    public void CMD_Bar(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "switch@trevor@bar", "exit_loop_bartender");
    }

    [Command("necessidades")]
    public void CMD_Necessidades(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@on_toilet", "trev_on_toilet_loop");
    }

    [Command("meth")]
    public void CMD_Meth(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)AnimationFlags.StopOnLastFrame, "switch@trevor@trev_smoking_meth", "trev_smoking_meth_loop");
    }

    [Command("mijar")]
    public void CMD_Mijar(Client player)
    {
        if (!ChecarPlayerAnim(player))
            return;

        API.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "misscarsteal2peeing", "peeing_loop");
    }
    #endregion

    // aqui pra baixo é lixo que tem q ser revisto
    [Command("obj")]
    public void obj(Client player, string bone, int id)
    {
        var obj = API.createObject(id, new Vector3(), new Vector3(), API.getEntityDimension(player));
        API.attachEntityToEntity(obj, player, bone, new Vector3(), new Vector3());
    }

    [Command("lobj")]
    public void lobj(Client player)
    {
        foreach (var obj in API.getAllObjects())
            API.deleteEntity(obj);
    }

    [Command("anim")]
    public void Anim(Client player, string animDict, string animName)
    {
        API.playPlayerAnimation(player, 1, animDict, animName);
    }

    [Command("ac")]
    public void acc(Client player, int slot, int drawable, int texture)
    {
        API.setPlayerAccessory(player, slot, drawable, texture);
    }

    [Command("rc")]
    public void crc(Client player)
    {
        API.clearPlayerAccessory(player, 0);
        API.clearPlayerAccessory(player, 1);
        API.clearPlayerAccessory(player, 2);
    }

    [Command("t")]
    public void t(Client player, int oi, int eae)
    {
        if (player.isInVehicle)
            API.setVehicleMod(player.vehicle, oi, eae);
    }

    [Command("c")]
    public void c(Client player, int oi, int eae, int eae2)
    {
        API.setPlayerClothes(player, oi, eae, eae2);
    }

    [Command("teste")]
    public void testecmd(Client player)
    {
        API.sendChatMessageToPlayer(player, "GTAO_FACE_FEATURES_1: " + API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_1"));

    }

    [Command("cao")]
    public void cao(Client player, int cachorroid)
    {
        Vector3 pos = API.getEntityPosition(player);
        NetHandle myDog;

        if (API.hasEntitySyncedData(player, "mydog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") != 0)
            {
                API.deleteEntity(API.getEntitySyncedData(player, "mydog"));
                API.setEntitySyncedData(player, "mydog", null);
                API.setEntitySyncedData(player, "TemDog", 0);
                API.setEntitySyncedData(player, "DogSentado", 0);
                return;
            }
        }

        if (cachorroid == 1) myDog = API.createPed(API.pedNameToModel("Husky"), pos, 0);
        else if (cachorroid == 2) myDog = API.createPed(API.pedNameToModel("Rottweiler"), pos, 0);
        else if (cachorroid == 3) myDog = API.createPed(API.pedNameToModel("Pug"), pos, 0);
        else if (cachorroid == 4) myDog = API.createPed(API.pedNameToModel("Retriever"), pos, 0);
        else if (cachorroid == 5) myDog = API.createPed(API.pedNameToModel("Shepherd"), pos, 0);
        else if (cachorroid == 6) myDog = API.createPed(API.pedNameToModel("Westy"), pos, 0);
        else myDog = API.createPed(API.pedNameToModel("Westy"), pos, 0);

        API.setEntityPositionFrozen(myDog, false);
        API.setEntityInvincible(myDog, false);

        API.setEntitySyncedData(player, "TemDog", 1);
        API.setEntitySyncedData(player, "mydog", myDog);
        API.setEntitySyncedData(player, "DogSentado", 0);
    }

    [Command("caosenta")]
    public void cao_senta(Client player)
    {

        if (API.hasEntitySyncedData(player, "mydog"))
        {
            if (API.getEntitySyncedData(player, "TemDog") != 0)
            {
                if (API.getEntitySyncedData(player, "DogSentado") == 0)
                {
                    API.setEntitySyncedData(player, "DogSentado", 1);
                    API.playPedScenario(API.getEntitySyncedData(player, "mydog"), "WORLD_DOG_SITTING_ROTTWEILER");
                    API.stopPedAnimation(API.getEntitySyncedData(player, "mydog"));
                    EnviarMensagemSucesso(player, "Cachorro sentado.");
                    API.setEntityPositionFrozen(API.getEntitySyncedData(player, "mydog"), true);
                }
                else
                {
                    EnviarMensagemSucesso(player, "Cachorro te seguindo.");
                    API.setEntitySyncedData(player, "DogSentado", 0);
                    API.stopPedAnimation(API.getEntitySyncedData(player, "mydog"));
                    API.setEntityPositionFrozen(API.getEntitySyncedData(player, "mydog"), false);
                }
                return;
            }
        }
        EnviarMensagemErro(player, "Você não está com seu cachorro.");
    }
    /* ***********************************************************
     * PERSONAGEM CUSTOM (GTA:O) By: Freeze 
     ********************************************************** */
    private void CreateCustomPers(Client player)
    {
        int PlayerID = API.getEntitySyncedData(player, "id");
        var str = string.Format("INSERT INTO personagem_custom (PersID) VALUES ({0})", PlayerID);
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();
        return;
    }
    private void SaveCustomPlayer(Client player)
    {
        if (API.getEntitySyncedData(player, "GTAO_HAS_CHARACTER_DATA") != true)
            return;

        int PlayerID = API.getEntitySyncedData(player, "id");
        var strPers = string.Format(@"UPDATE personagem_custom SET GTAO_SHAPE_FIRST_ID={0}, GTAO_SHAPE_SECOND_ID={1}, GTAO_SKIN_FIRST_ID={2}, 
                GTAO_SKIN_SECOND_ID={3}, GTAO_SHAPE_MIX={4}, GTAO_SKIN_MIX={5}, GTAO_HAIR_COLOR={6}, GTAO_HAIR_HIGHLIGHT_COLOR={7},
                GTAO_EYE_COLOR={8}, GTAO_EYEBROWS={9}, GTAO_EYEBROWS_COLOR={10}, GTAO_MAKEUP_COLOR={11}, GTAO_LIPSTICK_COLOR={12},
                GTAO_EYEBROWS_COLOR2={13}, GTAO_MAKEUP_COLOR2={14}, GTAO_LIPSTICK_COLOR2={15}, GTAO_HAIR_STYLE={16}, GTAO_FACE_FEATURES_1={17}, GTAO_FACE_FEATURES_2={18}
                ,GTAO_FACE_FEATURES_3={19},GTAO_FACE_FEATURES_4={20},GTAO_FACE_FEATURES_5={21},GTAO_FACE_FEATURES_6={22},GTAO_FACE_FEATURES_7={23},GTAO_FACE_FEATURES_8={24}
                ,GTAO_FACE_FEATURES_9={25},GTAO_FACE_FEATURES_10={26},GTAO_FACE_FEATURES_11={27},GTAO_FACE_FEATURES_12={28},GTAO_FACE_FEATURES_13={29},GTAO_FACE_FEATURES_14={30}
                ,GTAO_FACE_FEATURES_15={31},GTAO_FACE_FEATURES_16={32},GTAO_FACE_FEATURES_17={33},GTAO_FACE_FEATURES_18={34},GTAO_FACE_FEATURES_19={35},GTAO_FACE_FEATURES_20={36}
                ,GTAO_FACE_FEATURES_21={37} WHERE PersID = {38}",
                API.getEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID"),
                API.getEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID"),
                API.getEntitySyncedData(player, "GTAO_SKIN_FIRST_ID"),
                API.getEntitySyncedData(player, "GTAO_SKIN_SECOND_ID"),
                aFlt(API.getEntitySyncedData(player, "GTAO_SHAPE_MIX")),
                aFlt(API.getEntitySyncedData(player, "GTAO_SKIN_MIX")),
                API.getEntitySyncedData(player, "GTAO_HAIR_COLOR"),
                API.getEntitySyncedData(player, "GTAO_HAIR_HIGHLIGHT_COLOR"),
                API.getEntitySyncedData(player, "GTAO_EYE_COLOR"),
                API.getEntitySyncedData(player, "GTAO_EYEBROWS"),
                API.getEntitySyncedData(player, "GTAO_EYEBROWS_COLOR"),
                API.getEntitySyncedData(player, "GTAO_MAKEUP_COLOR"),
                API.getEntitySyncedData(player, "GTAO_LIPSTICK_COLOR"),
                API.getEntitySyncedData(player, "GTAO_EYEBROWS_COLOR2"),
                API.getEntitySyncedData(player, "GTAO_MAKEUP_COLOR2"),
                API.getEntitySyncedData(player, "GTAO_LIPSTICK_COLOR2"),
                API.getEntitySyncedData(player, "GTAO_HAIR_STYLE"),

                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_1")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_2")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_3")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_4")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_5")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_6")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_7")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_8")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_9")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_10")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_11")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_12")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_13")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_14")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_15")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_16")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_17")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_18")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_19")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_20")),
                aFlt(API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_21")),
                PlayerID
                );
        var cmdPers = new MySqlCommand(strPers, bancodados);
        cmdPers.ExecuteNonQuery();

        //API.consoleOutput(strPers);

        var strPers2 = string.Format(@"UPDATE personagem_custom SET Clothes_3={0}, Clothes_3_T={1}, Clothes_4={2}, Clothes_4_T={3}, 
                Clothes_8={4}, Clothes_8_T={5}, Clothes_11={6}, Clothes_11_T={7}, Clothes_6={8}, Clothes_6_T={9} WHERE PersID={10}",
                API.getEntitySyncedData(player, "Clothes_3"),
                API.getEntitySyncedData(player, "Clothes_3_T"),
                API.getEntitySyncedData(player, "Clothes_4"),
                API.getEntitySyncedData(player, "Clothes_4_T"),
                API.getEntitySyncedData(player, "Clothes_8"),
                API.getEntitySyncedData(player, "Clothes_8_T"),
                API.getEntitySyncedData(player, "Clothes_11"),
                API.getEntitySyncedData(player, "Clothes_11_T"),
                API.getEntitySyncedData(player, "Clothes_6"),
                API.getEntitySyncedData(player, "Clothes_6_T"),
                PlayerID
                );
        var cmdPers2 = new MySqlCommand(strPers2, bancodados);
        cmdPers2.ExecuteNonQuery();
    }

    private void LoadCustomPlayer(Client player)
    {
        int PlayerID = API.getEntitySyncedData(player, "id");

        var str = string.Format("SELECT * FROM personagem_custom WHERE PersID = {0} LIMIT 1", PlayerID);
        var cmd = new MySqlCommand(str, bancodados);
        var lendoPers = cmd.ExecuteReader();

        //API.consoleOutput("DEBUG 1: "+ str);

        if (lendoPers.HasRows)
        {
            lendoPers.Read();

            API.setEntitySyncedData(player, "GTAO_HAS_CHARACTER_DATA", true);

            API.setEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID", isInt(lendoPers["GTAO_SHAPE_FIRST_ID"].ToString()));
            API.setEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID", isInt(lendoPers["GTAO_SHAPE_SECOND_ID"].ToString()));
            API.setEntitySyncedData(player, "GTAO_SKIN_FIRST_ID", isInt(lendoPers["GTAO_SKIN_FIRST_ID"].ToString()));
            API.setEntitySyncedData(player, "GTAO_SKIN_SECOND_ID", isInt(lendoPers["GTAO_SKIN_SECOND_ID"].ToString()));
            API.setEntitySyncedData(player, "GTAO_SHAPE_MIX", isFloat(lendoPers["GTAO_SHAPE_MIX"].ToString()));
            API.setEntitySyncedData(player, "GTAO_SKIN_MIX", isFloat(lendoPers["GTAO_SKIN_MIX"].ToString()));
            API.setEntitySyncedData(player, "GTAO_HAIR_COLOR", isInt(lendoPers["GTAO_HAIR_COLOR"].ToString()));
            API.setEntitySyncedData(player, "GTAO_HAIR_HIGHLIGHT_COLOR", isInt(lendoPers["GTAO_HAIR_HIGHLIGHT_COLOR"].ToString()));
            API.setEntitySyncedData(player, "GTAO_EYE_COLOR", isInt(lendoPers["GTAO_EYE_COLOR"].ToString()));
            API.setEntitySyncedData(player, "GTAO_EYEBROWS", isInt(lendoPers["GTAO_EYEBROWS"].ToString()));

            //API.setEntitySyncedData(ent, "GTAO_MAKEUP", 0); // No lipstick by default. 
            //API.setEntitySyncedData(ent, "GTAO_LIPSTICK", 0); // No makeup by default.

            API.setEntitySyncedData(player, "GTAO_EYEBROWS_COLOR", isInt(lendoPers["GTAO_EYEBROWS_COLOR"].ToString()));
            API.setEntitySyncedData(player, "GTAO_MAKEUP_COLOR", isInt(lendoPers["GTAO_MAKEUP_COLOR"].ToString()));
            API.setEntitySyncedData(player, "GTAO_LIPSTICK_COLOR", isInt(lendoPers["GTAO_LIPSTICK_COLOR"].ToString()));
            API.setEntitySyncedData(player, "GTAO_EYEBROWS_COLOR2", isInt(lendoPers["GTAO_EYEBROWS_COLOR2"].ToString()));
            API.setEntitySyncedData(player, "GTAO_MAKEUP_COLOR2", isInt(lendoPers["GTAO_MAKEUP_COLOR2"].ToString()));
            API.setEntitySyncedData(player, "GTAO_LIPSTICK_COLOR2", isInt(lendoPers["GTAO_LIPSTICK_COLOR2"].ToString()));

            API.setEntitySyncedData(player, "GTAO_HAIR_STYLE", isInt(lendoPers["GTAO_HAIR_STYLE"].ToString()));

            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_1", isInt(lendoPers["GTAO_FACE_FEATURES_1"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_2", isInt(lendoPers["GTAO_FACE_FEATURES_2"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_3", isInt(lendoPers["GTAO_FACE_FEATURES_3"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_4", isInt(lendoPers["GTAO_FACE_FEATURES_4"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_5", isInt(lendoPers["GTAO_FACE_FEATURES_5"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_6", isInt(lendoPers["GTAO_FACE_FEATURES_6"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_7", isInt(lendoPers["GTAO_FACE_FEATURES_7"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_8", isInt(lendoPers["GTAO_FACE_FEATURES_8"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_9", isInt(lendoPers["GTAO_FACE_FEATURES_9"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_10", isInt(lendoPers["GTAO_FACE_FEATURES_10"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_11", isInt(lendoPers["GTAO_FACE_FEATURES_11"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_12", isInt(lendoPers["GTAO_FACE_FEATURES_12"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_13", isInt(lendoPers["GTAO_FACE_FEATURES_13"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_14", isInt(lendoPers["GTAO_FACE_FEATURES_14"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_15", isInt(lendoPers["GTAO_FACE_FEATURES_15"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_16", isInt(lendoPers["GTAO_FACE_FEATURES_16"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_17", isInt(lendoPers["GTAO_FACE_FEATURES_17"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_18", isInt(lendoPers["GTAO_FACE_FEATURES_18"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_19", isInt(lendoPers["GTAO_FACE_FEATURES_19"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_20", isInt(lendoPers["GTAO_FACE_FEATURES_20"].ToString()));
            API.setEntitySyncedData(player, "GTAO_FACE_FEATURES_21", isInt(lendoPers["GTAO_FACE_FEATURES_21"].ToString()));

            API.setEntitySyncedData(player, "Clothes_3", isInt(lendoPers["Clothes_3"].ToString()));
            API.setEntitySyncedData(player, "Clothes_3_T", isInt(lendoPers["Clothes_3_T"].ToString()));
            API.setEntitySyncedData(player, "Clothes_4", isInt(lendoPers["Clothes_4"].ToString()));
            API.setEntitySyncedData(player, "Clothes_4_T", isInt(lendoPers["Clothes_4_T"].ToString()));
            API.setEntitySyncedData(player, "Clothes_6", isInt(lendoPers["Clothes_6"].ToString()));
            API.setEntitySyncedData(player, "Clothes_6_T", isInt(lendoPers["Clothes_6_T"].ToString()));
            API.setEntitySyncedData(player, "Clothes_8", isInt(lendoPers["Clothes_8"].ToString()));
            API.setEntitySyncedData(player, "Clothes_8_T", isInt(lendoPers["Clothes_8_T"].ToString()));
            API.setEntitySyncedData(player, "Clothes_11", isInt(lendoPers["Clothes_11"].ToString()));
            API.setEntitySyncedData(player, "Clothes_11_T", isInt(lendoPers["Clothes_11_T"].ToString()));

            API.triggerClientEvent(player, "LoadSkinPers", player,
                 API.getEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID"),
                 API.getEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID"),
                 API.getEntitySyncedData(player, "GTAO_SKIN_FIRST_ID"),
                 API.getEntitySyncedData(player, "GTAO_SKIN_SECOND_ID"),
                 API.getEntitySyncedData(player, "GTAO_SHAPE_MIX"),
                 API.getEntitySyncedData(player, "GTAO_SKIN_MIX"),
                 API.getEntitySyncedData(player, "GTAO_HAIR_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_HAIR_HIGHLIGHT_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_EYE_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_EYEBROWS"),
                 API.getEntitySyncedData(player, "GTAO_EYEBROWS_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_MAKEUP_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_LIPSTICK_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_EYEBROWS_COLOR2"),
                 API.getEntitySyncedData(player, "GTAO_MAKEUP_COLOR2"),
                 API.getEntitySyncedData(player, "GTAO_LIPSTICK_COLOR2"),
                 API.getEntitySyncedData(player, "GTAO_HAIR_STYLE"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_1"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_2"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_3"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_4"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_5"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_6"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_7"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_8"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_9"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_10"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_11"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_12"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_13"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_14"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_15"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_16"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_17"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_18"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_19"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_20"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_21"),
                 true
                 );

            //API.sendNotificationToPlayer(player, "~g~Personagem custom carregado.");

            updatePlayerFace(player);
            SetarMinhasRoupas(player);
        }
        else
        {
            API.setEntitySyncedData(player, "GTAO_HAS_CHARACTER_DATA", false);
        }

        lendoPers.Close();



    }

    public void initializePedFace(Client ent, int PersOnline)
    {
        if (PersOnline == 0) API.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", false);
        else API.setEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA", true);

        API.setEntitySyncedData(ent, "Clothes_3", 0);
        API.setEntitySyncedData(ent, "Clothes_4", 0);
        API.setEntitySyncedData(ent, "Clothes_6", 0);
        API.setEntitySyncedData(ent, "Clothes_8", 0);
        API.setEntitySyncedData(ent, "Clothes_11", 0);

        API.setEntitySyncedData(ent, "Clothes_3_T", 0);
        API.setEntitySyncedData(ent, "Clothes_4_T", 0);
        API.setEntitySyncedData(ent, "Clothes_6_T", 0);
        API.setEntitySyncedData(ent, "Clothes_8_T", 0);
        API.setEntitySyncedData(ent, "Clothes_11_T", 0);

        API.setEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID", 0);
        API.setEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID", 0);
        API.setEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID", 0);
        API.setEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID", 0);
        API.setEntitySyncedData(ent, "GTAO_SHAPE_MIX", 0f);
        API.setEntitySyncedData(ent, "GTAO_SKIN_MIX", 0f);
        API.setEntitySyncedData(ent, "GTAO_HAIR_COLOR", 0);
        API.setEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR", 0);
        API.setEntitySyncedData(ent, "GTAO_EYE_COLOR", 0);
        API.setEntitySyncedData(ent, "GTAO_EYEBROWS", 0);

        //API.setEntitySyncedData(ent, "GTAO_MAKEUP", 0); // No lipstick by default. 
        //API.setEntitySyncedData(ent, "GTAO_LIPSTICK", 0); // No makeup by default.

        API.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR", 0);
        API.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR", 0);
        API.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR", 0);
        API.setEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2", 0);
        API.setEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2", 0);
        API.setEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2", 0);

        API.setEntitySyncedData(ent, "GTAO_HAIR_STYLE", 0);

        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_1", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_2", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_3", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_4", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_5", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_6", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_7", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_8", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_9", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_10", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_11", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_12", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_13", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_14", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_15", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_16", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_17", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_18", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_19", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_20", 0);
        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_21", 0);

        var list = new float[21];
        for (var i = 0; i < 21; i++)
        {
            list[i] = 0f;
        }

        API.setEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST", list);
    }

    public void updatePlayerFace(NetHandle player)
    {
        API.triggerClientEventForAll("UPDATE_CHARACTER_1", player);
        API.triggerClientEventForAll("UPDATE_CHARACTER", player);
    }
    //============================================================================
    public ColShape Loja_Camisetas;
    public ColShape Loja_Bermudas;
    public ColShape Loja_Calcas;
    public ColShape Loja_Casmole;
    public ColShape Loja_Calcados;

    public void CriarIconsLojaDeRoupas()
    {
        //======================================================================================
        //Binco - Ganton
        Blip BincoGanton = API.createBlip(new Vector3(81.90916, -1391.526, 29.38765));
        API.setBlipSprite(BincoGanton, 73);
        API.setBlipName(BincoGanton, "Binco");
        API.setBlipShortRange(BincoGanton, true);

        //Camisetas
        API.createTextLabel("Camisas", new Vector3(77.51466, -1396.471, 29), 8f, 0.6f);
        API.createMarker(1, new Vector3(77.51466, -1396.471, 28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 50, 153, 204);
        Loja_Camisetas = API.createCylinderColShape(new Vector3(77.51466, -1396.471, 29.37614), 1f, 1f);
        Loja_Camisetas.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para comprar uma camiseta.");
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 1);
            }
        };
        Loja_Camisetas.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 0);
            }
        };

        //Bermudas
        API.createTextLabel("Bermudas", new Vector3(80.22811, -1398.718, 29), 8f, 0.6f);
        API.createMarker(1, new Vector3(80.22811, -1398.718, 28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 50, 153, 204);
        Loja_Bermudas = API.createCylinderColShape(new Vector3(80.22811, -1398.718, 29.37614), 1f, 1f);
        Loja_Bermudas.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para comprar uma bermuda.");
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 2);
            }
        };
        Loja_Bermudas.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 0);
            }
        };

        //Calças
        API.createTextLabel("Calças", new Vector3(69.86195, -1398.801, 29), 8f, 0.6f);
        API.createMarker(1, new Vector3(69.86195, -1398.801, 28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 50, 153, 204);
        Loja_Calcas = API.createCylinderColShape(new Vector3(69.86195, -1398.801, 29.38488), 1f, 1f);
        Loja_Calcas.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para comprar uma calça.");
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 3);
            }
        };
        Loja_Calcas.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 0);
            }
        };

        //Casacos
        API.createTextLabel("Casacos", new Vector3(71.2989, -1400.278, 29.37615), 8f, 0.6f);
        API.createMarker(1, new Vector3(71.2989, -1400.278, 28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 50, 153, 204);
        Loja_Casmole = API.createCylinderColShape(new Vector3(71.2989, -1400.278, 29.37615), 1f, 1f);
        Loja_Casmole.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para comprar um casaco.");
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 4);
            }
        };
        Loja_Casmole.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 0);
            }
        };

        //Calçados
        API.createTextLabel("Calçados", new Vector3(81.40653, -1396.411, 29.37614), 8f, 0.6f);
        API.createMarker(1, new Vector3(81.40653, -1396.411, 28), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 50, 153, 204);
        Loja_Calcados = API.createCylinderColShape(new Vector3(81.40653, -1396.411, 29.37614), 1f, 1f);
        Loja_Calcados.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para comprar um calçado.");
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 5);
            }
        };
        Loja_Calcados.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.setEntitySyncedData(player.handle, "CheckpointRoupas", 0);
            }
        };

        //=====
    }

    [Command("criar")]
    public void criarpers(Client player)
    {
        int PlayerID = API.getEntitySyncedData(player, "id");
        var str = string.Format("SELECT * FROM personagem_custom WHERE PersID = {0}", PlayerID);
        var cmd = new MySqlCommand(str, bancodados);
        var lendoPers = cmd.ExecuteReader();

        if (lendoPers.HasRows)
        {
            API.sendNotificationToPlayer(player, "~r~[ERRO] ~w~Você não pode recriar o seu personagem.");
            lendoPers.Close();
            return;
        }
        else
        {
            lendoPers.Close();

            initializePedFace(player, 1);
            updatePlayerFace(player);
            API.setEntitySyncedData(player, "GTAO_HAS_CHARACTER_DATA", true);

            //==========================
            API.triggerClientEvent(player, "LoadSkinPers", player,
                 API.getEntitySyncedData(player, "GTAO_SHAPE_FIRST_ID"),
                 API.getEntitySyncedData(player, "GTAO_SHAPE_SECOND_ID"),
                 API.getEntitySyncedData(player, "GTAO_SKIN_FIRST_ID"),
                 API.getEntitySyncedData(player, "GTAO_SKIN_SECOND_ID"),
                 API.getEntitySyncedData(player, "GTAO_SHAPE_MIX"),
                 API.getEntitySyncedData(player, "GTAO_SKIN_MIX"),
                 API.getEntitySyncedData(player, "GTAO_HAIR_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_HAIR_HIGHLIGHT_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_EYE_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_EYEBROWS"),
                 API.getEntitySyncedData(player, "GTAO_EYEBROWS_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_MAKEUP_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_LIPSTICK_COLOR"),
                 API.getEntitySyncedData(player, "GTAO_EYEBROWS_COLOR2"),
                 API.getEntitySyncedData(player, "GTAO_MAKEUP_COLOR2"),
                 API.getEntitySyncedData(player, "GTAO_LIPSTICK_COLOR2"),
                 API.getEntitySyncedData(player, "GTAO_HAIR_STYLE"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_1"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_2"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_3"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_4"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_5"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_6"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_7"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_8"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_9"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_10"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_11"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_12"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_13"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_14"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_15"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_16"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_17"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_18"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_19"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_20"),
                 API.getEntitySyncedData(player, "GTAO_FACE_FEATURES_21"),
                 API.getEntitySyncedData(player, "GTAO_HAS_CHARACTER_DATA")
                 );
            //==========================

            API.triggerClientEvent(player, "CREATE_PERS", 1);

            API.setEntityPosition(player, new Vector3(402.9244, -996.588, -99.00025));
            API.setEntityRotation(player, new Vector3(0, 0, 180));
            API.freezePlayer(player, true);
            API.triggerClientEvent(player, "criarcamera", new Vector3(402.9244, -999.491, -99.00404), new Vector3(402.9244, -996.288, -99.00025));

            /**********************************************
                     * PERSONAGEM CUSTOM
            * *******************************************/
            CreateCustomPers(player);
        }

    }

    [Command("skin")]
    public void ChangeSkinCommand(Client sender, PedHash model)
    {
        API.setPlayerSkin(sender, model);
        API.sendNativeToPlayer(sender, 0x45EEE61580806D63, sender.handle);
    }

    public void SetarMinhasRoupas(Client player)
    {
        int Clothes_3 = API.getEntitySyncedData(player, "Clothes_3"),
            Clothes_3_T = API.getEntitySyncedData(player, "Clothes_3_T"),
            Clothes_4 = API.getEntitySyncedData(player, "Clothes_4"),
            Clothes_4_T = API.getEntitySyncedData(player, "Clothes_4_T"),
            Clothes_6 = API.getEntitySyncedData(player, "Clothes_6"),
            Clothes_6_T = API.getEntitySyncedData(player, "Clothes_6_T"),
            Clothes_8 = API.getEntitySyncedData(player, "Clothes_8"),
            Clothes_8_T = API.getEntitySyncedData(player, "Clothes_8_T"),
            Clothes_11 = API.getEntitySyncedData(player, "Clothes_11"),
            Clothes_11_T = API.getEntitySyncedData(player, "Clothes_11_T");

        API.setPlayerClothes(player, 3, Clothes_3, Clothes_3_T);
        API.setPlayerClothes(player, 4, Clothes_4, Clothes_4_T);
        API.setPlayerClothes(player, 6, Clothes_6, Clothes_6_T);
        API.setPlayerClothes(player, 8, Clothes_8, Clothes_8_T);
        API.setPlayerClothes(player, 11, Clothes_11, Clothes_11_T);

        updatePlayerFace(player);
    }
    /*public void SetarPedRoupas(Ped ped, int a3, int a3_t, int a4, int a4_t, int a6, int a6_t, int a8, int a8_t, int a11, int a11_t)
    {
        API.setPlayerClothes(ped, 3, a3, a3_t);
        API.setPlayerClothes(ped, 4, a4, a4_t);
        API.setPlayerClothes(ped, 6, a6, a6_t);
        API.setPlayerClothes(ped, 8, a8, a8_t);
        API.setPlayerClothes(ped, 11, a11, a11_t);
    }*/
    public void TrocaClotheSlot(Client player, int index, int modelo, int textura, int comprou = 0)
    {
        if (comprou == 1)
        {
            switch (index)
            {
                case 3:
                    API.setEntitySyncedData(player.handle, "Clothes_3", modelo);
                    API.setEntitySyncedData(player.handle, "Clothes_3_T", textura);
                    break;
                case 4:
                    API.setEntitySyncedData(player.handle, "Clothes_4", modelo);
                    API.setEntitySyncedData(player.handle, "Clothes_4_T", textura);
                    break;
                case 6:
                    API.setEntitySyncedData(player.handle, "Clothes_6", modelo);
                    API.setEntitySyncedData(player.handle, "Clothes_6_T", textura);
                    break;
                case 8:
                    API.setEntitySyncedData(player.handle, "Clothes_8", modelo);
                    API.setEntitySyncedData(player.handle, "Clothes_8_T", textura);
                    break;
                case 11:
                    API.setEntitySyncedData(player.handle, "Clothes_11", modelo);
                    API.setEntitySyncedData(player.handle, "Clothes_11_T", textura);
                    break;

            }
        }
        API.setPlayerClothes(player, index, modelo, textura); // Top
    }

    public void Comprando_Calca(Client player, int modelo, int textura = 0, int comprou = 0)
    {
        if (modelo < 0 || modelo > 61) { API.sendNotificationToPlayer(player, "~b~Escolha uma camisa de 0 a 61."); return; }
        switch (modelo)
        {
            case 0: TrocaClotheSlot(player, 4, 0, textura, comprou); break;
            case 1: TrocaClotheSlot(player, 4, 1, textura, comprou); break;
            case 2: TrocaClotheSlot(player, 4, 3, textura, comprou); break;
            case 3: TrocaClotheSlot(player, 4, 4, textura, comprou); break;
            case 4: TrocaClotheSlot(player, 4, 5, textura, comprou); break;
            case 5: TrocaClotheSlot(player, 4, 7, textura, comprou); break;
            case 6: TrocaClotheSlot(player, 4, 8, textura, comprou); break;
            case 7: TrocaClotheSlot(player, 4, 9, textura, comprou); break;
            case 8: TrocaClotheSlot(player, 4, 10, textura, comprou); break;
            case 9: TrocaClotheSlot(player, 4, 13, textura, comprou); break;
            case 10: TrocaClotheSlot(player, 4, 19, textura, comprou); break;
            case 11: TrocaClotheSlot(player, 4, 20, textura, comprou); break;
            case 12: TrocaClotheSlot(player, 4, 22, textura, comprou); break;
            case 13: TrocaClotheSlot(player, 4, 23, textura, comprou); break;
            case 14: TrocaClotheSlot(player, 4, 24, textura, comprou); break;
            case 15: TrocaClotheSlot(player, 4, 25, textura, comprou); break;
            case 16: TrocaClotheSlot(player, 4, 26, textura, comprou); break;
            case 17: TrocaClotheSlot(player, 4, 27, textura, comprou); break;
            case 18: TrocaClotheSlot(player, 4, 28, textura, comprou); break;
            case 19: TrocaClotheSlot(player, 4, 29, textura, comprou); break;
            case 20: TrocaClotheSlot(player, 4, 30, textura, comprou); break;
            case 21: TrocaClotheSlot(player, 4, 32, textura, comprou); break;
            case 22: TrocaClotheSlot(player, 4, 34, textura, comprou); break;
            case 23: TrocaClotheSlot(player, 4, 35, textura, comprou); break;
            case 24: TrocaClotheSlot(player, 4, 36, textura, comprou); break;
            case 25: TrocaClotheSlot(player, 4, 37, textura, comprou); break;
            case 26: TrocaClotheSlot(player, 4, 38, textura, comprou); break;
            case 27: TrocaClotheSlot(player, 4, 39, textura, comprou); break;
            case 28: TrocaClotheSlot(player, 4, 41, textura, comprou); break;
            case 29: TrocaClotheSlot(player, 4, 43, textura, comprou); break;
            case 30: TrocaClotheSlot(player, 4, 45, textura, comprou); break;
            case 31: TrocaClotheSlot(player, 4, 46, textura, comprou); break;
            case 32: TrocaClotheSlot(player, 4, 47, textura, comprou); break;
            case 33: TrocaClotheSlot(player, 4, 48, textura, comprou); break;
            case 34: TrocaClotheSlot(player, 4, 49, textura, comprou); break;
            case 35: TrocaClotheSlot(player, 4, 50, textura, comprou); break;
            case 36: TrocaClotheSlot(player, 4, 51, textura, comprou); break;
            case 37: TrocaClotheSlot(player, 4, 52, textura, comprou); break;
            case 38: TrocaClotheSlot(player, 4, 53, textura, comprou); break;
            case 39: TrocaClotheSlot(player, 4, 55, textura, comprou); break;
            case 40: TrocaClotheSlot(player, 4, 58, textura, comprou); break;
            case 41: TrocaClotheSlot(player, 4, 60, textura, comprou); break;
            case 42: TrocaClotheSlot(player, 4, 63, textura, comprou); break;
            case 43: TrocaClotheSlot(player, 4, 64, textura, comprou); break;
            case 44: TrocaClotheSlot(player, 4, 65, textura, comprou); break;
            case 45: TrocaClotheSlot(player, 4, 66, textura, comprou); break;
            case 46: TrocaClotheSlot(player, 4, 67, textura, comprou); break;
            case 47: TrocaClotheSlot(player, 4, 68, textura, comprou); break;
            case 48: TrocaClotheSlot(player, 4, 69, textura, comprou); break;
            case 49: TrocaClotheSlot(player, 4, 70, textura, comprou); break;
            case 50: TrocaClotheSlot(player, 4, 71, textura, comprou); break;
            case 51: TrocaClotheSlot(player, 4, 73, textura, comprou); break;
            case 52: TrocaClotheSlot(player, 4, 75, textura, comprou); break;
            case 53: TrocaClotheSlot(player, 4, 76, textura, comprou); break;
            case 54: TrocaClotheSlot(player, 4, 77, textura, comprou); break;
            case 55: TrocaClotheSlot(player, 4, 78, textura, comprou); break;
            case 56: TrocaClotheSlot(player, 4, 79, textura, comprou); break;
            case 57: TrocaClotheSlot(player, 4, 80, textura, comprou); break;
            case 58: TrocaClotheSlot(player, 4, 81, textura, comprou); break;
            case 59: TrocaClotheSlot(player, 4, 82, textura, comprou); break;
            case 60: TrocaClotheSlot(player, 4, 83, textura, comprou); break;
            case 61: TrocaClotheSlot(player, 4, 85, textura, comprou); break;
        }
    }

    public void Comprando_Bermuda(Client player, int modelo, int textura = 0, int comprou = 0)
    {
        if (modelo < 0 || modelo > 12) { API.sendNotificationToPlayer(player, "~b~Escolha uma camisa de 0 a 12."); return; }
        switch (modelo)
        {

            case 0: TrocaClotheSlot(player, 4, 6, textura, comprou); break;
            case 1: TrocaClotheSlot(player, 4, 12, textura, comprou); break;
            case 2: TrocaClotheSlot(player, 4, 14, textura, comprou); break;
            case 3: TrocaClotheSlot(player, 4, 15, textura, comprou); break;
            case 4: TrocaClotheSlot(player, 4, 16, textura, comprou); break;
            case 5: TrocaClotheSlot(player, 4, 17, textura, comprou); break;
            case 6: TrocaClotheSlot(player, 4, 18, textura, comprou); break;
            case 7: TrocaClotheSlot(player, 4, 21, textura, comprou); break;
            case 8: TrocaClotheSlot(player, 4, 42, textura, comprou); break;
            case 9: TrocaClotheSlot(player, 4, 54, textura, comprou); break;
            case 10: TrocaClotheSlot(player, 4, 56, textura, comprou); break;
            case 11: TrocaClotheSlot(player, 4, 61, textura, comprou); break;
            case 12: TrocaClotheSlot(player, 4, 62, textura, comprou); break;
        }
    }

    public void Comprando_Camisa(Client player, int modelo, int textura = 0, int comprou = 0)
    {
        if (modelo < 0 || modelo > 58) { API.sendNotificationToPlayer(player, "~b~Escolha uma camisa de 0 a 95."); return; }
        switch (modelo)
        {

            case 0:
                TrocaClotheSlot(player, 11, 1, textura, comprou);
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou);
                break;
            case 1:
                TrocaClotheSlot(player, 11, 5, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 5, 0, comprou); // TORSO
                break;
            case 2:
                TrocaClotheSlot(player, 11, 8, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 8, 0, comprou); // TORSO
                break;
            case 3:
                TrocaClotheSlot(player, 11, 9, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 4:
                TrocaClotheSlot(player, 11, 12, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 5:
                TrocaClotheSlot(player, 11, 13, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 6:
                TrocaClotheSlot(player, 11, 14, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 7:
                TrocaClotheSlot(player, 11, 16, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 8:
                TrocaClotheSlot(player, 11, 17, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 5, 0, comprou); // TORSO
                break;
            case 9:
                TrocaClotheSlot(player, 11, 18, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 10:
                TrocaClotheSlot(player, 11, 22, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 11:
                TrocaClotheSlot(player, 11, 22, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 12:
                TrocaClotheSlot(player, 11, 26, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 11, 0, comprou); // TORSO
                break;
            case 13:
                TrocaClotheSlot(player, 11, 33, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 14:
                TrocaClotheSlot(player, 11, 34, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 15:
                TrocaClotheSlot(player, 11, 36, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 5, 0, comprou); // TORSO
                break;
            case 16:
                TrocaClotheSlot(player, 11, 38, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 8, 0, comprou); // TORSO
                break;
            case 17:
                TrocaClotheSlot(player, 11, 39, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 18:
                TrocaClotheSlot(player, 11, 41, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 19:
                TrocaClotheSlot(player, 11, 42, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 11, 0, comprou); // TORSO
                break;
            case 20:
                TrocaClotheSlot(player, 11, 43, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 11, 0, comprou); // TORSO
                break;
            case 21:
                TrocaClotheSlot(player, 11, 44, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 22:
                TrocaClotheSlot(player, 11, 47, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 23:
                TrocaClotheSlot(player, 11, 53, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 24:
                TrocaClotheSlot(player, 11, 56, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 25:
                TrocaClotheSlot(player, 11, 63, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 26:
                TrocaClotheSlot(player, 11, 67, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 27:
                TrocaClotheSlot(player, 11, 71, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 28:
                TrocaClotheSlot(player, 11, 73, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 29:
                TrocaClotheSlot(player, 11, 80, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 30:
                TrocaClotheSlot(player, 11, 81, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 31:
                TrocaClotheSlot(player, 11, 82, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 32:
                TrocaClotheSlot(player, 11, 83, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 33:
                TrocaClotheSlot(player, 11, 85, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 34:
                TrocaClotheSlot(player, 11, 93, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 35:
                TrocaClotheSlot(player, 11, 94, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 36:
                TrocaClotheSlot(player, 11, 95, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 11, 0, comprou); // TORSO
                break;
            case 37:
                TrocaClotheSlot(player, 11, 97, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 38:
                TrocaClotheSlot(player, 11, 98, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 39:
                TrocaClotheSlot(player, 11, 105, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 11, 0, comprou); // TORSO
                break;
            case 40:
                TrocaClotheSlot(player, 11, 107, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 41:
                TrocaClotheSlot(player, 11, 123, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 42:
                TrocaClotheSlot(player, 11, 126, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 43:
                TrocaClotheSlot(player, 11, 128, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 11, 0, comprou); // TORSO
                break;
            case 44:
                TrocaClotheSlot(player, 11, 131, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 45:
                TrocaClotheSlot(player, 11, 132, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 46:
                TrocaClotheSlot(player, 11, 133, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 47:
                TrocaClotheSlot(player, 11, 135, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 48:
                TrocaClotheSlot(player, 11, 146, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 49:
                TrocaClotheSlot(player, 11, 148, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 50:
                TrocaClotheSlot(player, 11, 152, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 51:
                TrocaClotheSlot(player, 11, 178, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 52:
                TrocaClotheSlot(player, 11, 193, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 0, 0, comprou); // TORSO
                break;
            case 53:
                TrocaClotheSlot(player, 11, 194, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 54:
                TrocaClotheSlot(player, 11, 196, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 55:
                TrocaClotheSlot(player, 11, 201, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 3, 0, comprou); // TORSO
                break;
            case 56:
                TrocaClotheSlot(player, 11, 202, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 5, 0, comprou); // TORSO
                break;
            case 57:
                TrocaClotheSlot(player, 11, 205, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 58:
                TrocaClotheSlot(player, 11, 51, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
        }
    }

    public void Comprando_Calcado(Client player, int modelo, int textura = 0, int comprou = 0)
    {
        if (modelo < 0 || modelo > 54) { API.sendNotificationToPlayer(player, "~b~Escolha um calçado de 0 a 54."); return; }
        switch (modelo)
        {
            case 0: TrocaClotheSlot(player, 6, 0, textura, comprou); break;
            case 1: TrocaClotheSlot(player, 6, 1, textura, comprou); break;
            case 2: TrocaClotheSlot(player, 6, 2, textura, comprou); break;
            case 3: TrocaClotheSlot(player, 6, 3, textura, comprou); break;
            case 4: TrocaClotheSlot(player, 6, 4, textura, comprou); break;
            case 5: TrocaClotheSlot(player, 6, 5, textura, comprou); break;
            case 6: TrocaClotheSlot(player, 6, 6, textura, comprou); break;
            case 7: TrocaClotheSlot(player, 6, 7, textura, comprou); break;
            case 8: TrocaClotheSlot(player, 6, 8, textura, comprou); break;
            case 9: TrocaClotheSlot(player, 6, 9, textura, comprou); break;
            case 10: TrocaClotheSlot(player, 6, 10, textura, comprou); break;
            case 11: TrocaClotheSlot(player, 6, 11, textura, comprou); break;

            case 12: TrocaClotheSlot(player, 6, 12, textura, comprou); break;
            case 13: TrocaClotheSlot(player, 6, 14, textura, comprou); break;
            case 14: TrocaClotheSlot(player, 6, 15, textura, comprou); break;
            case 15: TrocaClotheSlot(player, 6, 16, textura, comprou); break;
            case 16: TrocaClotheSlot(player, 6, 17, textura, comprou); break;
            case 17: TrocaClotheSlot(player, 6, 18, textura, comprou); break;
            case 18: TrocaClotheSlot(player, 6, 19, textura, comprou); break;
            case 19: TrocaClotheSlot(player, 6, 20, textura, comprou); break;

            case 20: TrocaClotheSlot(player, 6, 21, textura, comprou); break;
            case 21: TrocaClotheSlot(player, 6, 22, textura, comprou); break;
            case 22: TrocaClotheSlot(player, 6, 23, textura, comprou); break;
            case 23: TrocaClotheSlot(player, 6, 24, textura, comprou); break;
            case 24: TrocaClotheSlot(player, 6, 25, textura, comprou); break;
            case 25: TrocaClotheSlot(player, 6, 26, textura, comprou); break;
            case 26: TrocaClotheSlot(player, 6, 27, textura, comprou); break;
            case 27: TrocaClotheSlot(player, 6, 28, textura, comprou); break;
            case 28: TrocaClotheSlot(player, 6, 29, textura, comprou); break;
            case 29: TrocaClotheSlot(player, 6, 30, textura, comprou); break;

            case 30: TrocaClotheSlot(player, 6, 31, textura, comprou); break;
            case 31: TrocaClotheSlot(player, 6, 32, textura, comprou); break;
            case 32: TrocaClotheSlot(player, 6, 35, textura, comprou); break;
            case 33: TrocaClotheSlot(player, 6, 36, textura, comprou); break;
            case 34: TrocaClotheSlot(player, 6, 37, textura, comprou); break;
            case 35: TrocaClotheSlot(player, 6, 38, textura, comprou); break;
            case 36: TrocaClotheSlot(player, 6, 39, textura, comprou); break;
            case 37: TrocaClotheSlot(player, 6, 40, textura, comprou); break;

            case 38: TrocaClotheSlot(player, 6, 41, textura, comprou); break;
            case 39: TrocaClotheSlot(player, 6, 42, textura, comprou); break;
            case 40: TrocaClotheSlot(player, 6, 43, textura, comprou); break;
            case 41: TrocaClotheSlot(player, 6, 44, textura, comprou); break;
            case 42: TrocaClotheSlot(player, 6, 46, textura, comprou); break;

            case 43: TrocaClotheSlot(player, 6, 47, textura, comprou); break;
            case 44: TrocaClotheSlot(player, 6, 48, textura, comprou); break;
            case 45: TrocaClotheSlot(player, 6, 49, textura, comprou); break;
            case 46: TrocaClotheSlot(player, 6, 50, textura, comprou); break;

            case 47: TrocaClotheSlot(player, 6, 51, textura, comprou); break;
            case 48: TrocaClotheSlot(player, 6, 52, textura, comprou); break;
            case 49: TrocaClotheSlot(player, 6, 53, textura, comprou); break;
            case 50: TrocaClotheSlot(player, 6, 54, textura, comprou); break;
            case 51: TrocaClotheSlot(player, 6, 55, textura, comprou); break;
            case 52: TrocaClotheSlot(player, 6, 56, textura, comprou); break;
            case 53: TrocaClotheSlot(player, 6, 57, textura, comprou); break;
            case 54: TrocaClotheSlot(player, 6, 58, textura, comprou); break;
        }
    }

    public void Comprando_CasacoMoletom(Client player, int modelo, int textura = 0, int comprou = 0)
    {
        if (modelo < 0 || modelo > 58) { API.sendNotificationToPlayer(player, "~b~Escolha uma camisa de 0 a 95."); return; }
        switch (modelo)
        {
            case 0:
                TrocaClotheSlot(player, 11, 48, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 1:
                TrocaClotheSlot(player, 11, 50, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 2:
                TrocaClotheSlot(player, 11, 54, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 3:
                TrocaClotheSlot(player, 11, 57, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 4:
                TrocaClotheSlot(player, 11, 61, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 5:
                TrocaClotheSlot(player, 11, 75, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);

                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 6:
                TrocaClotheSlot(player, 11, 78, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou);


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 7:
                TrocaClotheSlot(player, 11, 79, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 8:
                TrocaClotheSlot(player, 11, 84, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 9:
                TrocaClotheSlot(player, 11, 86, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 10:
                TrocaClotheSlot(player, 11, 87, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 11:
                TrocaClotheSlot(player, 11, 89, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 12:
                TrocaClotheSlot(player, 11, 90, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 13:
                TrocaClotheSlot(player, 11, 96, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 14:
                TrocaClotheSlot(player, 11, 110, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 15:
                TrocaClotheSlot(player, 11, 111, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 16:
                TrocaClotheSlot(player, 11, 121, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 17:
                TrocaClotheSlot(player, 11, 125, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 18://SecuroServ
                TrocaClotheSlot(player, 11, 129, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 19:
                TrocaClotheSlot(player, 11, 134, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 20:
                TrocaClotheSlot(player, 11, 143, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;


            case 21:
                TrocaClotheSlot(player, 11, 147, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 22:
                TrocaClotheSlot(player, 11, 150, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 23:
                TrocaClotheSlot(player, 11, 153, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 24:
                TrocaClotheSlot(player, 11, 154, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 25:
                TrocaClotheSlot(player, 11, 171, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 26:
                TrocaClotheSlot(player, 11, 182, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 27:
                TrocaClotheSlot(player, 11, 184, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 28:
                TrocaClotheSlot(player, 11, 187, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 29:
                TrocaClotheSlot(player, 11, 188, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 30:
                TrocaClotheSlot(player, 11, 190, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 31:
                TrocaClotheSlot(player, 11, 200, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;

            case 32:
                TrocaClotheSlot(player, 11, 203, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 33:
                TrocaClotheSlot(player, 11, 204, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 1, 0, comprou); // TORSO
                break;
            case 34:
                TrocaClotheSlot(player, 11, 205, textura, comprou); // Top
                TrocaClotheSlot(player, 8, 15, 0, comprou); // Undershirt


                TrocaClotheSlot(player, 3, 5, 0, comprou); // TORSO
                break;
        }
    }

    #region Emprego_Taxista
    public ColShape Infotaxi;
    public int taxistaOnDuty = 0;

    public void CriarEmprego_Taxista()
    {
        API.createMarker(0, new Vector3(895.6638, -179.3419, 74.70026), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 255, 255, 0);
        Blip Taxi = API.createBlip(new Vector3(895.6638, -179.3419, 74.70034), 30f);
        API.setBlipSprite(Taxi, 56);
        API.setBlipColor(Taxi, 5);
        API.setBlipName(Taxi, "Downtown Cab Co.");
        Infotaxi = API.createCylinderColShape(new Vector3(895.6638, -179.3419, 73.70026), 3f, 3f);
        API.createTextLabel("Pressione Y para se tornar um taxista.", new Vector3(895.6638, -179.3419, 74.70026), 8f, 0.6f);

        Infotaxi.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                if (GetPlayerJob(player) == 0)
                    API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para se tornar um taxista.");
                else if (GetPlayerJob(player) != 1)
                    API.sendNotificationToPlayer(player, "~b~Você deve sair do seu atual emprego antes de se tornar um taxista.");

                API.triggerClientEvent(player, "status_marker_job", 1);
            }
        };
        Infotaxi.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.triggerClientEvent(player, "status_marker_job", 0);
            }
        };
    }

    //CLIENTES - COMANDOS PARA QUALQUER UM
    [Command("taxi")]
    public void chamandoTaxi(Client player)
    {
        if (API.getEntityDimension(player) != 0) { API.sendNotificationToPlayer(player, "~r~Você não pode pedir um taxi de dentro de um interior."); return; }
        if (API.getEntitySyncedData(player, "TAXISTA_ACEITOU") != "") { API.sendNotificationToPlayer(player, "~b~Você já tem uma chamada aceita, aguarde a chegada do taxista."); return; }
        API.setEntitySyncedData(player, "TAXI_ESPERANDO", 1);

        int pass_name = recuperarIDPorClient(player);

        if (taxistaOnDuty == 0)
            API.sendPictureNotificationToPlayer(player, "~s~Desculpe-nos, mas estamos sem taxistas disponiveis no momento.. Tente daqui a pouco.", "CHAR_TAXI", 5, 4, "Downtown Cab. Co.", "Sem motoristas disponiveis");
        else
        {
            API.sendPictureNotificationToPlayer(player, "~s~Você solicitou um taxi. Aguarde até que um taxista aceite sua chamada.", "CHAR_TAXI", 5, 2, "Downtown Cab. Co.", "Solicitando um taxi");
            //Enviar mensagem para Taxistas online.
            foreach (var driver in API.getAllPlayers())
            {
                if (API.getEntitySyncedData(driver, "TAXI_ONDUTY") == 1)
                {
                    API.sendPictureNotificationToPlayer(driver, "~s~Recebemos uma nova chamada, alguém está disponivel? (Use '/aceitarcorrida " + pass_name + "' para aceitar).", "CHAR_TAXI", 12, 2, "Downtown Cab. Co.", "Nova chamada");

                }
            }
        }
        //=======================================
    }
    [Command("cancelartaxi")]
    public void cancelandoTaxi(Client player)
    {
        if (API.getEntitySyncedData(player, "TAXISTA_ACEITOU") == "") { API.sendNotificationToPlayer(player, "~r~Você não tem uma chamada aceita."); return; }
        ClienteCancelaCorrida(player);
        //=======================================
    }

    //COMANDO PARA QUEM É TAXISTA
    [Command("aceitarcorrida", "USE: /aceitarcorrida [Playerid]")]
    public void aceitandoTaxi(Client player, String idOrName)
    {
        if (!API.isPlayerInAnyVehicle(player)) { API.sendNotificationToPlayer(player, "~r~Você não esta em um veículo."); return; }
        if (player.vehicle.model != -956048545) { API.sendNotificationToPlayer(player, "~r~Você não esta em um taxi."); return; }
        if (GetPlayerJob(player) != 1) { API.sendNotificationToPlayer(player, "~r~Você não é um taxista."); return; }
        if (DutyStatus(player) == 0) { API.sendNotificationToPlayer(player, "~r~Você não está em serviço."); return; }
        if (API.getEntitySyncedData(player, "TAXI_ACEITOU_COR") != "") { API.sendNotificationToPlayer(player, "~r~Você já aceitou uma chamada. (Use /cancelar se necessário)"); return; }

        var target = findPlayer(player, idOrName, false);

        if (API.getEntitySyncedData(target, "TAXI_ESPERANDO") == 1)
        {
            double senderxcoords, senderycoords;
            senderxcoords = API.getEntityPosition(target).X;
            senderycoords = API.getEntityPosition(target).Y;

            var motor_id = recuperarIDPorClient(player);

            API.setEntitySyncedData(player, "TAXI_ACEITOU_COR", target);
            API.setEntitySyncedData(target, "TAXI_ESPERANDO", 0);
            API.setEntitySyncedData(target, "TAXISTA_ACEITOU", motor_id);

            API.triggerClientEvent(player, "markonmap", senderxcoords, senderycoords);
            API.sendPictureNotificationToPlayer(target, "~s~Um taxista aceitou sua chamada, aguarde no local até a chegada do veículo.", "CHAR_TAXI", 5, 2, "Downtown Cab. Co.", "Taxista a caminho");

        }
        else
            API.sendNotificationToPlayer(player, "~r~Este jogador não solicitou um taxi, ou a chamada já foi aceita.");
        //=======================================

    }
    [Command("corrida", "USE: /corrida [valor]")]
    public void corrida(Client player, int valor)
    {
        if (!API.isPlayerInAnyVehicle(player)) { API.sendNotificationToPlayer(player, "~r~Você não esta em um veículo."); return; }
        if (player.vehicle.model != -956048545) { API.sendNotificationToPlayer(player, "~r~Você não esta em um taxi."); return; }

        if (GetPlayerJob(player) == 0)
        {
            API.sendNotificationToPlayer(player, "~r~Você não é um taxista.");
        }
        else
        {
            if (valor <= 0 || valor >= 11)
            {
                API.sendNotificationToPlayer(player, "~r~A tarifa deve ser entre 1 e 10.");
            }
            else
            {
                if (DutyStatus(player) == 0)
                    TogDutyStatus(player, 1, valor);
                else
                    TogDutyStatus(player, 0);
            }
        }
    }

    private void Taxi_OnPlayerDisconnectedHandler(Client player)
    {
        if (DutyStatus(player) == 1) TogDutyStatus(player, 0);

        if (API.getEntitySyncedData(player, "TAXISTA_ACEITOU") != "") ClienteCancelaCorrida(player);
    }
    private void Taxi_OnPlayerConnectedHandler(Client player)
    {
        API.setEntitySyncedData(player, "TAXI_ESPERANDO", 0);
        API.setEntitySyncedData(player, "TAXISTA_ACEITOU", "");
    }
    private void Taxi_OnPlayerEnterVehicle(Client player, NetHandle vehicle)
    {
        var my_name = recuperarIDPorClient(player);

        if (API.getPlayerVehicleSeat(player) != -1)
        {
            foreach (var driver in API.getAllPlayers())
            {
                if (API.getPlayerVehicle(driver) == vehicle && API.getPlayerVehicleSeat(driver) == -1 && API.getEntitySyncedData(driver, "TAXI_ONDUTY") == 1
                    && API.getEntitySyncedData(driver, "TAXI_ONDUTY") == my_name)
                {
                    API.sendNotificationToPlayer(player, "~b~Você entrou no taxi.");
                    API.sendNotificationToPlayer(driver, "~g~Você pegou um passageiro.");

                    API.setEntitySyncedData(player, "TAXI_ESPERANDO", 0);
                    API.setEntitySyncedData(player, "TAXISTA_ACEITOU", "");
                }
            }
        }
    }
    private void Taxi_OnPlayerExitVehicleHandler(Client player, NetHandle vehicle)
    {
        var my_name = recuperarIDPorClient(player);

        if (DutyStatus(player) == 1) TogDutyStatus(player, 0);


        if (API.getPlayerVehicleSeat(player) != -1)
        {
            foreach (var driver in API.getAllPlayers())
            {
                if (API.getPlayerVehicle(driver) == vehicle && API.getPlayerVehicleSeat(driver) == -1 && API.getEntitySyncedData(driver, "TAXI_ONDUTY") == 1
                    && API.getEntitySyncedData(driver, "TAXI_ONDUTY") == my_name)
                {
                    API.sendNotificationToPlayer(player, "~s~Você saiu do taxi.");
                    API.sendNotificationToPlayer(driver, "~b~O passageiro saiu do veículo.");

                    API.setEntitySyncedData(driver, "TAXI_PASSAGEIRO", "");
                    API.setEntitySyncedData(driver, "TAXI_ACEITOU_COR", "");

                    API.setEntitySyncedData(player, "TAXI_ESPERANDO", 0);
                    API.setEntitySyncedData(player, "TAXISTA_ACEITOU", "");
                }
            }
        }
    }

    public void ClienteCancelaCorrida(Client player)
    {
        var pass_name = recuperarIDPorClient(player);

        API.sendPictureNotificationToPlayer(player, "~s~Você cancelou o pedido do taxi.", "CHAR_TAXI", 5, 2, "Downtown Cab. Co.", "Cancelando chamada");
        //Enviar mensagem para Taxistas online.
        if (API.getEntitySyncedData(player, "TAXISTA_ACEITOU") != "")
        {
            foreach (var taxistaname in API.getAllPlayers())
            {
                if (String.ReferenceEquals(API.getEntitySyncedData(player, "TAXISTA_ACEITOU"), findPlayer(player, API.getPlayerName(taxistaname), false)))
                {
                    API.setEntitySyncedData(taxistaname, "TAXI_ACEITOU_COR", "");
                    API.setEntitySyncedData(taxistaname, "TAXISTA_ACEITOU", "");
                    API.sendPictureNotificationToPlayer(taxistaname, "~s~O seu passageiro cancelou o pedido.", "CHAR_TAXI", 5, 2, "Downtown Cab. Co.", "Chamada cancelada");
                }
            }
        }

        API.setEntitySyncedData(player, "TAXI_ESPERANDO", 0);
        API.setEntitySyncedData(player, "TAXISTA_ACEITOU", "");
    }

    public int GetPlayerJob(Client player)
    {
        int PlayerJob;
        if (API.hasEntitySyncedData(player, "Job"))
            PlayerJob = API.getEntitySyncedData(player, "Job");
        else
            PlayerJob = 0;

        API.sendNotificationToPlayer(player, Convert.ToString(API.getEntitySyncedData(player, "Job")));

        return PlayerJob;
    }

    public int DutyStatus(Client player)
    {
        int status;
        if (API.hasEntitySyncedData(player, "TAXI_ONDUTY"))
            status = API.getEntitySyncedData(player, "TAXI_ONDUTY");
        else
            status = 0;

        return status;
    }

    public void TogDutyStatus(Client player, int status, int valor = 0)
    {
        switch (status)
        {
            case 1: //Entrando em Serviço
                API.setEntitySyncedData(player, "TAXI_ONDUTY", 1);
                API.setEntitySyncedData(player, "TAXI_PASSAGEIRO", "");
                API.setEntitySyncedData(player, "TAXI_TARIFA", valor);
                API.setEntitySyncedData(player, "TAXI_ACEITOU_COR", "");
                API.sendNotificationToPlayer(player, "~b~Você entrou em serviço como taxista com o taximetro em $" + valor + ".");
                taxistaOnDuty++;

                break;

            case 0: //Saindo de Serviço

                if (API.getEntitySyncedData(player, "TAXI_ACEITOU_COR") != "")
                {
                    foreach (var passageiro in API.getAllPlayers())
                    {
                        if (API.getEntitySyncedData(player, "TAXI_ACEITOU_COR") == API.getPlayerName(passageiro))
                        {
                            API.setEntitySyncedData(passageiro, "TAXISTA_ACEITOU", "");
                            API.sendPictureNotificationToPlayer(passageiro, "~s~Desculpe-nos mas, o taxista que havia aceitado sua chamada saiu de serviço..~n~Chame um novo taxi.", "CHAR_TAXI", 5, 2, "Downtown Cab. Co.", "Taxista saiu de serviço");
                        }
                    }
                }

                API.setEntitySyncedData(player, "TAXI_ONDUTY", 0);
                API.setEntitySyncedData(player, "TAXI_PASSAGEIRO", "");
                API.setEntitySyncedData(player, "TAXI_TARIFA", 0);
                API.setEntitySyncedData(player, "TAXI_ACEITOU_COR", "");
                API.sendNotificationToPlayer(player, "~r~Você saiu de serviço.");
                taxistaOnDuty--;

                break;
        }
        return;
    }
    #endregion

    #region Emprego_MotoristaDeOnibus
    public ColShape Infoonibus;

    List<ColShape> Rota_1 = new List<ColShape>();
    List<ColShape> Rota_2 = new List<ColShape>();
    List<ColShape> Rota_3 = new List<ColShape>();
    List<Vector3> Linha_1 = new List<Vector3>();
    List<Vector3> Linha_2 = new List<Vector3>();
    List<Vector3> Linha_3 = new List<Vector3>();

    public void CriandoEmprego_MotoristaDeOnibus()
    {

        //Criando Rota 1
        Linha_1.Add(new Vector3(402.0585, -713.2081, 28.20688));
        Linha_1.Add(new Vector3(395.0054, -992.9345, 28.27768));
        Linha_1.Add(new Vector3(498.1435, -1281.496, 28.19415));
        Linha_1.Add(new Vector3(415.8489, -1666.946, 28.15584));
        Linha_1.Add(new Vector3(123.0442, -1547.092, 28.22022));
        Linha_1.Add(new Vector3(1.919736, -1357.441, 28.23541));
        Linha_1.Add(new Vector3(-71.5151, -1088.662, 25.60266));
        Linha_1.Add(new Vector3(109.6711, -599.0474, 30.55489));
        Linha_1.Add(new Vector3(461.9726, -605.5356, 27.49556));
        //Criando Rota 2
        Linha_2.Add(new Vector3(500.7567, -738.2877, 23.77528));
        Linha_2.Add(new Vector3(496.3747, -986.3292, 26.50582));
        Linha_2.Add(new Vector3(498.2406, -1283.756, 28.19729));
        Linha_2.Add(new Vector3(952.9802, -1448.7, 30.10205));
        Linha_2.Add(new Vector3(1184.242, -1055.266, 41.22021));
        Linha_2.Add(new Vector3(1183.306, -585.0261, 63.08186));
        Linha_2.Add(new Vector3(970.8248, -156.2356, 72.58362));
        Linha_2.Add(new Vector3(542.4201, 85.17707, 95.11423));
        Linha_2.Add(new Vector3(274.3727, 180.0495, 103.52));
        Linha_2.Add(new Vector3(14.39861, 224.6789, 107.5463));
        Linha_2.Add(new Vector3(-145.9097, -303.2822, 38.10226));
        Linha_2.Add(new Vector3(-156.0191, -714.054, 33.58944));
        Linha_2.Add(new Vector3(324.75, -866.8904, 28.2034));
        Linha_2.Add(new Vector3(-461.5771, -612.7409, 27.49703));
        //Criando Rota 3
        Linha_3.Add(new Vector3(307.9688, -763.5667, 29.22817));
        Linha_3.Add(new Vector3(114.9007, -784.7877, 31.32485));
        Linha_3.Add(new Vector3(-349.2952, -648.8773, 31.86956));
        Linha_3.Add(new Vector3(-694.4044, -648.0569, 30.9858));
        Linha_3.Add(new Vector3(-1140.604, -900.1768, 5.73105));
        Linha_3.Add(new Vector3(-1000.755, -839.4423, 14.53939));
        Linha_3.Add(new Vector3(-807.8377, -1064.391, 12.12456));
        Linha_3.Add(new Vector3(-579.4692, -962.3931, 22.85282));
        Linha_3.Add(new Vector3(-171.4426, -1149.9, 23.15698));
        Linha_3.Add(new Vector3(101.0201, -1046.558, 29.25047));
        Linha_3.Add(new Vector3(354.4747, -1063.715, 29.39353));
        Linha_3.Add(new Vector3(410.4438, -740.5577, 29.17308));
        Linha_3.Add(new Vector3(460.2626, -619.1757, 28.49592));

        //Criando ColShape das Rotas
        for (int i = 0; i < Linha_1.Count; i++)
            Rota_1.Add(API.createCylinderColShape(Linha_1[i], 4f, 3f));

        for (int i = 0; i < Linha_2.Count; i++)
            Rota_2.Add(API.createCylinderColShape(Linha_2[i], 4f, 3f));

        for (int i = 0; i < Linha_3.Count; i++)
            Rota_3.Add(API.createCylinderColShape(Linha_3[i], 4f, 3f));

        //Criando Icone, BLIP, TextLabel e Checkpoint no terminal de onibus
        API.createMarker(0, new Vector3(434.5667, -646.7364, 28.73334), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 255, 255, 0);
        Blip Onibus = API.createBlip(new Vector3(434.5667, -646.7364, 28.73334));
        API.setBlipSprite(Onibus, 513);
        API.setBlipColor(Onibus, 0);
        API.setBlipName(Onibus, "Dashound bus center.");
        API.setBlipShortRange(Onibus, true);
        Infoonibus = API.createCylinderColShape(new Vector3(434.5667, -646.7364, 28.73334), 2f, 3f);
        API.createTextLabel("Pressione Y para interagir com o emprego de Motorista de Onibus.", new Vector3(434.5667, -646.7364, 28.73334), 8f, 0.6f);

        Infoonibus.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.triggerClientEvent(player, "status_marker_job", 0);
            }
        };
        Infoonibus.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                if (GetPlayerJob(player) == 0)
                    API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para se tornar um motorista de onibus.");
                else if (GetPlayerJob(player) == 2)
                    API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para iniciar seu trabalho como motorista.");
                else
                    API.sendNotificationToPlayer(player, "~b~Você deve sair do seu atual emprego antes de se tornar um motorista de onibus.");

                API.triggerClientEvent(player, "status_marker_job", 2);
            }
        };
    }

    private void OnEntityEnterColShapeHandler(ColShape shape, NetHandle entity)
    {
        Client player;
        if ((player = API.getPlayerFromHandle(entity)) != null)
        {
            int TaEmPosto = 0;
            for(int i = 0; i < PostoDeGasolina.Count; i++)
            {
                if (shape == PostoDeGasolina[i]){
                    TaEmPosto = 1;
                    break;
                }
            }
            if (TaEmPosto == 1)
            {
                if (API.isPlayerInAnyVehicle(player))
                {
                    var vehh = recuperarVeiculo(player.vehicle);
                    if (vehh != null)
                    {
                        if (!API.hasEntitySyncedData(player.handle, "EmPostoDeGasolina"))
                        {
                            EnviarMensagemSucesso(player, "Pressione Y para abastecer seu veículo.");
                            API.setEntitySyncedData(player.handle, "EmPostoDeGasolina", 1);
                        }

                        EnviarMensagemSucesso(player, "Pressione Y para abastecer seu veículo.");
                        API.setEntitySyncedData(player.handle, "EmPostoDeGasolina", 1);
                    }
                    else
                        API.sendNotificationToPlayer(player, "ERRO #00032");
                }
            }

            if (API.getEntitySyncedData(player.handle, "Job") == 2)
            {
                if (API.hasEntitySyncedData(player.handle, "Rota") && API.getEntitySyncedData(player.handle, "Rota") != 0)
                {
                    if (API.isPlayerInAnyVehicle(player))
                    {
                        if (API.getEntitySyncedData(player.handle, "Rota") > 0 && API.getEntitySyncedData(player.handle, "Parte") > 0)
                        {
                            if (API.getPlayerVehicle(player) != API.getEntitySyncedData(player.handle, "CarJob"))
                            {
                                API.sendNotificationToPlayer(player, "~r~Você não está em seu onibus de serviço.");
                                return;
                            }
                        }

                        int atualParte;
                        atualParte = API.getEntitySyncedData(player.handle, "Parte");
                        int AtualTeste = atualParte - 1;
                        int rota = API.getEntitySyncedData(player.handle, "Rota");

                        if (API.getEntitySyncedData(player.handle, "Rota") > 0)
                        {
                            if (API.getEntitySyncedData(player.handle, "Parte") < GetTamanhoRota(rota)
                                && (API.getEntitySyncedData(player.handle, "Rota") == 1 && shape == Rota_1[AtualTeste])
                                || (API.getEntitySyncedData(player.handle, "Rota") == 2 && shape == Rota_2[AtualTeste])
                                || (API.getEntitySyncedData(player.handle, "Rota") == 3 && shape == Rota_3[AtualTeste])
                                )
                            {
                                int Tamanho = GetTamanhoRota(rota);

                                API.setEntitySyncedData(player.handle, "Parte", atualParte + 1);

                                API.triggerClientEvent(player, "remove_job_marker", "bus");
                                API.triggerClientEvent(player, "bus_checkpoint", GetNextPoint(player, rota, atualParte), atualParte, player);
                                API.triggerClientEvent(player, "update_bus_job", true, atualParte, Tamanho);

                                API.setPlayerVelocity(player, new Vector3(0, 0, 0));
                            }
                            else if ((shape == Rota_2[AtualTeste] || shape == Rota_3[AtualTeste] || shape == Rota_1[AtualTeste]) && API.getEntitySyncedData(player.handle, "Parte") == GetTamanhoRota(rota))
                            {
                                API.sendNotificationToPlayer(player, "~b~Você concluiu a rota.");

                                API.triggerClientEvent(player, "finalize_job_bus", "bus");
                                API.triggerClientEvent(player, "update_bus_job", false, atualParte, 0);

                                if (API.hasEntitySyncedData(player.handle, "CarJob"))
                                {
                                    NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
                                    API.deleteEntity(CarDel);
                                    API.setEntitySyncedData(player.handle, "CarJob", null);
                                }

                                //Motorista de Onibus
                                API.setEntitySyncedData(player.handle, "Rota", 0);
                                API.setEntitySyncedData(player.handle, "Parte", 0);
                            }
                            //=====
                        }
                    }
                }
            }
            if (API.getEntitySyncedData(player.handle, "Job") == 3)//Los Santos Services
            {
                if (API.hasEntitySyncedData(player.handle, "Rota") && API.getEntitySyncedData(player.handle, "Rota") != 0)
                {
                    int atualParte;
                    atualParte = API.getEntitySyncedData(player.handle, "Parte");
                    int AtualTeste = atualParte - 1;
                    int rota = API.getEntitySyncedData(player.handle, "Rota");

                    EnviarMensagemSucesso(player, "Parte: "+ atualParte + " | AtualTeste: "+ AtualTeste);

                    if (API.getEntitySyncedData(player.handle, "Rota") > 0)
                    {
                        if (API.getEntitySyncedData(player.handle, "Parte") < GetTamanhoRota_LSS(rota)
                            && (API.getEntitySyncedData(player.handle, "Rota") == 1 && shape == Rota_1[AtualTeste])
                            || (API.getEntitySyncedData(player.handle, "Rota") == 2 && shape == Rota_2[AtualTeste])
                            || (API.getEntitySyncedData(player.handle, "Rota") == 3 && shape == Rota_3[AtualTeste])
                            )
                        {
                            if (API.getEntitySyncedData(player.handle, "Rota") > 0 && API.getEntitySyncedData(player.handle, "Parte") > 0)
                            {
                                if (API.isPlayerInAnyVehicle(player))
                                {
                                    EnviarMensagemErro(player, "~r~Você deve estar fora do veículo.");
                                    return;
                                }
                            }

                            int Tamanho = GetTamanhoRota_LSS(rota);

                            API.setEntitySyncedData(player.handle, "Parte", atualParte + 1);

                            API.triggerClientEvent(player, "remove_job_marker", "bus");


                            API.setEntitySyncedData(player.handle, "NaoPodePararAnim", 1);

                            API.playPlayerScenario(player, "WORLD_HUMAN_CONST_DRILL");

                            API.delay(12000, true, () =>
                            {
                                API.triggerClientEvent(player, "bus_checkpoint", GetNextPoint_LSS(player, rota, atualParte), atualParte, player);
                                API.triggerClientEvent(player, "update_bus_job", true, atualParte, Tamanho);

                                API.stopPlayerAnimation(player);
                                API.setEntitySyncedData(player.handle, "NaoPodePararAnim", 0);
                            });

                            API.setPlayerVelocity(player, new Vector3(0, 0, 0));
                        }
                        else if ((shape == Rota_2[AtualTeste] || shape == Rota_3[AtualTeste] || shape == Rota_1[AtualTeste]) && API.getEntitySyncedData(player.handle, "Parte") == GetTamanhoRota_LSS(rota))
                        {
                            API.sendNotificationToPlayer(player, "~b~Você concluiu o serviço.");

                            API.triggerClientEvent(player, "finalize_job_bus", "bus");
                            API.triggerClientEvent(player, "update_bus_job", false, atualParte, 0);

                            if (API.hasEntitySyncedData(player.handle, "CarJob"))
                            {
                                NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
                                API.deleteEntity(CarDel);
                                API.setEntitySyncedData(player.handle, "CarJob", null);
                            }

                            //Motorista de Onibus
                            API.setEntitySyncedData(player.handle, "Rota", 0);
                            API.setEntitySyncedData(player.handle, "Parte", 0);
                        }
                        //=====
                    }
                }

            }
            if (API.getEntitySyncedData(player.handle, "AutoEscola_Teste") > 0)//Los Santos Services
            {
                int atualParte;
                atualParte = API.getEntitySyncedData(player.handle, "Parte");
                int AtualTeste = atualParte - 1;

                EnviarMensagemErro(player, "Auto escola, ponto atual: " + AtualTeste);

                if (API.getEntitySyncedData(player.handle, "Parte") < GetTamanhoRota_AutoEscola() && shape == Rota_AutoEscola[AtualTeste])
                {
                    int Tamanho = GetTamanhoRota_AutoEscola();

                    API.setEntitySyncedData(player.handle, "Parte", atualParte + 1);
                    switch (atualParte)
                    {
                        case 1:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: O limite pelas ruas da cidade é de 50Km/h.. Vire a direita.");
                            break;
                        case 2:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Vire a esquerda aqui.");
                            break;
                        case 3:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Ok, a direita agora, e lembre-se de respeitar a sinalização!");
                            break;
                        case 4:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Certo, vamos em frente.");
                            break;
                        case 5:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Reto novamente.");
                            break;
                        case 6:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Olhe bem para os lados antes de passar pelo cruzamento, vamos em frente..");
                            break;
                        case 7:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Siga reto e vá indo para direita, vamos virar logo ali na frente..");
                            break;
                        case 8:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Beleza, a direita agora.");
                            break;
                        case 9:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Com bastante atenção aqui.. Não vamos arrumar problemas perto da delegacia né?");
                            EnviarMensagemRadius(player, DISTANCIA_RP, "* Instrutor rí.", COR_ROLEPLAY);
                            break;
                        case 10:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Aqui é o departamento de policia, se precisar de uma ajuda policial, passe por aqui.");
                            break;
                        case 11:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Okey, a direita.");
                            break;
                        case 12:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Siga reto.. Repare que as vezes o GPS marca outro caminho ...");
                            API.sendChatMessageToPlayer(player, "... é por que o local que vamos, fica abaixo de alguma rua ou ponte.");
                            break;
                        case 13:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Ótimo, vire a próxima a direita.");
                            break;
                        case 14:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Reto.");
                            break;
                        case 15:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Aqui na esqueda é um dos Hospitais da cidade ...");
                            API.sendChatMessageToPlayer(player, "... você pode vir aqui quando precisar de cuidados médicos.");
                            break;
                        case 16:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Okey, a direita.");
                            break;
                        case 17:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Reto!");
                            break;
                        case 18:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Reto, por debaixo da ponte.");
                            break;
                        case 19:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: A esqueda agora, vamos pegar a estrada.");
                            break;
                        case 20:
                            API.setEntitySyncedData(player, "VelMax", 120);
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Pelas auto estradas de San Andreas, podemos andar um pouco mais rápido.. Até 120km/h.");
                            break;
                        case 21:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Repare o seu velocimetro mudando de cor, quando ele estiver no vermelho ...");
                            API.sendChatMessageToPlayer(player, "... ele gastará mais gasolina do que o normal.");
                            break;
                        case 22:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Se mantenha a direita, vamos pegar essa saída.");
                            break;
                        case 23:
                            API.setEntitySyncedData(player, "VelMax", 0);
                            API.sendChatMessageToPlayer(player, "Instrutor diz: E aqui estamos saindo da estrada, lembre-se que devemos voltar a ...");
                            API.sendChatMessageToPlayer(player, "... andar a no máximo 50km/h.. A direita, por baixo da ponte.");
                            break;
                        case 24:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Certo, vamos em frente.");
                            break;
                        case 25:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Já estamos na reta final!");
                            break;
                        case 26:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Vamos lá, em frente!");
                            break;
                        case 27:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Beleza, vamos fazer uma curva fechada agora.. Faça-a devagar e com cuidado..");
                            break;
                        case 28:
                            API.sendChatMessageToPlayer(player, "Instrutor diz: Boa! Pare ali em frente a auto escola.");
                            break;
                    }
                    API.triggerClientEvent(player, "remove_job_marker", "bus");
                    API.triggerClientEvent(player, "bus_checkpoint", GetNextPoint_AutoEscola(player, atualParte), 0);
                }
                else if (shape == Rota_AutoEscola[AtualTeste] && API.getEntitySyncedData(player.handle, "Parte") == GetTamanhoRota_AutoEscola())
                {
                    API.sendNotificationToPlayer(player, "~b~Você concluiu o teste da Auto Escola.");

                    API.setEntitySyncedData(player, "carLic", 1);

                    API.triggerClientEvent(player, "finalize_job_bus", "bus");

                    NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
                    API.deleteEntity(CarDel);
                    API.setEntitySyncedData(player.handle, "CarJob", null);

                    var pedcc = (NetHandle)API.getEntitySyncedData(player, "pedautoescola");
                    API.deleteEntity(pedcc);
                    API.resetEntitySyncedData(player, "pedautoescola");

                    //Motorista de Onibus
                    API.setEntitySyncedData(player.handle, "AutoEscola_Teste", 0);
                }
                //=====


            }
            //===
        }
    }

    [Command("sairemprego")]
    public void CMD_sairemprego(Client player)
    {
        API.setEntitySyncedData(player, "Job", 0);
        EnviarMensagemErro(player, "Você saiu do seu emprego.");
    }

    [Command("pararrota")]
    public void parando_rota(Client player)
    {
        if (API.hasEntitySyncedData(player.handle, "Rota"))
        {
            if (API.getEntitySyncedData(player.handle, "Rota") == 0)
            {
                API.sendNotificationToPlayer(player, "~r~Você não está em uma rota.");
                return;
            }
            API.sendNotificationToPlayer(player, "~r~Você cancelou a rota.");

            API.triggerClientEvent(player, "finalize_job_bus", "bus");
            API.triggerClientEvent(player, "update_bus_job", false, 0, 0);

            if (API.hasEntitySyncedData(player.handle, "CarJob"))
            {
                NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
                API.deleteEntity(CarDel);
                API.setEntitySyncedData(player.handle, "CarJob", null);
            }

            API.setEntitySyncedData(player.handle, "Rota", 0);
            API.setEntitySyncedData(player.handle, "Parte", 0);
        }
    }

    private void CarroEmprego_OnPlayerDisconnectedHandler(Client player)
    {
        if (API.hasEntitySyncedData(player.handle, "CarJob"))
        {
            NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
            API.deleteEntity(CarDel);
            API.resetEntitySyncedData(player.handle, "CarJob");
        }
        return;
    }

    public Vector3 GetNextPoint(Client player, int route, int nextpoint)
    {
        switch (route)
        {
            case 1: return Linha_1[nextpoint];
            case 2: return Linha_2[nextpoint];
            case 3: return Linha_3[nextpoint];
        }
        return new Vector3();
    }
    public int GetTamanhoRota(int rota)
    {
        int Tamanho = 0;
        switch (rota)
        {
            case 1: Tamanho = Rota_1.Count; break;
            case 2: Tamanho = Rota_2.Count; break;
            case 3: Tamanho = Rota_3.Count; break;
        }
        return Tamanho;
    }
    #endregion

    #region Emprego_Manutencao_City
    public ColShape Job_Manutencao;

    List<ColShape> Obras_1 = new List<ColShape>();
    List<ColShape> Obras_2 = new List<ColShape>();
    List<ColShape> Obras_3 = new List<ColShape>();
    List<Vector3> Manut_1 = new List<Vector3>();
    List<Vector3> Manut_2 = new List<Vector3>();
    List<Vector3> Manut_3 = new List<Vector3>();

    public void CriandoEmprego_ManutencaoCity()
    {

        //Criando Rota 1
        Manut_1.Add(new Vector3(402.0585, -713.2081, 28.20688));
        Manut_1.Add(new Vector3(395.0054, -992.9345, 28.27768));
        Manut_1.Add(new Vector3(498.1435, -1281.496, 28.19415));
        Manut_1.Add(new Vector3(415.8489, -1666.946, 28.15584));
        Manut_1.Add(new Vector3(123.0442, -1547.092, 28.22022));
        Manut_1.Add(new Vector3(1.919736, -1357.441, 28.23541));
        Manut_1.Add(new Vector3(-71.5151, -1088.662, 25.60266));
        Manut_1.Add(new Vector3(109.6711, -599.0474, 30.55489));
        Manut_1.Add(new Vector3(461.9726, -605.5356, 27.49556));
        //Criando Rota 2
        Manut_2.Add(new Vector3(500.7567, -738.2877, 23.77528));
        Manut_2.Add(new Vector3(496.3747, -986.3292, 26.50582));
        Manut_2.Add(new Vector3(498.2406, -1283.756, 28.19729));
        Manut_2.Add(new Vector3(952.9802, -1448.7, 30.10205));
        Manut_2.Add(new Vector3(1184.242, -1055.266, 41.22021));
        Manut_2.Add(new Vector3(1183.306, -585.0261, 63.08186));
        Manut_2.Add(new Vector3(970.8248, -156.2356, 72.58362));
        Manut_2.Add(new Vector3(542.4201, 85.17707, 95.11423));
        Manut_2.Add(new Vector3(274.3727, 180.0495, 103.52));
        Manut_2.Add(new Vector3(14.39861, 224.6789, 107.5463));
        Manut_2.Add(new Vector3(-145.9097, -303.2822, 38.10226));
        Manut_2.Add(new Vector3(-156.0191, -714.054, 33.58944));
        Manut_2.Add(new Vector3(324.75, -866.8904, 28.2034));
        Manut_2.Add(new Vector3(-461.5771, -612.7409, 27.49703));
        //Criando Rota 3
        Manut_3.Add(new Vector3(307.9688, -763.5667, 29.22817));
        Manut_3.Add(new Vector3(114.9007, -784.7877, 31.32485));
        Manut_3.Add(new Vector3(-349.2952, -648.8773, 31.86956));
        Manut_3.Add(new Vector3(-694.4044, -648.0569, 30.9858));
        Manut_3.Add(new Vector3(-1140.604, -900.1768, 5.73105));
        Manut_3.Add(new Vector3(-1000.755, -839.4423, 14.53939));
        Manut_3.Add(new Vector3(-807.8377, -1064.391, 12.12456));
        Manut_3.Add(new Vector3(-579.4692, -962.3931, 22.85282));
        Manut_3.Add(new Vector3(-171.4426, -1149.9, 23.15698));
        Manut_3.Add(new Vector3(101.0201, -1046.558, 29.25047));
        Manut_3.Add(new Vector3(354.4747, -1063.715, 29.39353));
        Manut_3.Add(new Vector3(410.4438, -740.5577, 29.17308));
        Manut_3.Add(new Vector3(460.2626, -619.1757, 28.49592));

        //Criando ColShape das Rotas
        for (int i = 0; i < Manut_1.Count; i++)
            Obras_1.Add(API.createCylinderColShape(Manut_1[i], 4f, 3f));

        for (int i = 0; i < Manut_2.Count; i++)
            Obras_2.Add(API.createCylinderColShape(Manut_2[i], 4f, 3f));

        for (int i = 0; i < Manut_3.Count; i++)
            Obras_3.Add(API.createCylinderColShape(Manut_3[i], 4f, 3f));

        //Criando Icone, BLIP, TextLabel e Checkpoint no terminal de onibus
        API.createMarker(0, new Vector3(-322.0825, -1545.806, 30.01991), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 255, 255, 0);
        Blip ServicosLS = API.createBlip(new Vector3(-322.0825, -1545.806, 31.01991));
        API.setBlipSprite(ServicosLS, 513);
        API.setBlipColor(ServicosLS, 0);
        API.setBlipName(ServicosLS, "L.S Services.");
        API.setBlipShortRange(ServicosLS, true);
        Job_Manutencao = API.createCylinderColShape(new Vector3(-322.0825, -1545.806, 31.01991), 2f, 3f);
        API.createTextLabel("[L.S Services]\nPressione Y para interagir com o emprego.", new Vector3(-322.0825, -1545.806, 31.01991), 8f, 0.6f);

        Job_Manutencao.onEntityExitColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                API.triggerClientEvent(player, "status_marker_job", 0);
            }
        };
        Job_Manutencao.onEntityEnterColShape += (shape, entity) =>
        {
            Client player;
            if ((player = API.getPlayerFromHandle(entity)) != null)
            {
                if (GetPlayerJob(player) == 0)
                    API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para trabalhar na LS Services.");
                else if (GetPlayerJob(player) == 2)
                    API.sendNotificationToPlayer(player, "~b~Pressione 'Y' para iniciar um serviço.");
                else
                    API.sendNotificationToPlayer(player, "~b~Você deve sair do seu atual emprego antes de trabalhar na LS Services.");

                API.triggerClientEvent(player, "status_marker_job", 3);
            }
        };
    }

    [Command("pararservico")]
    public void CMD_pararservico(Client player)
    {
        if (API.hasEntitySyncedData(player.handle, "Rota"))
        {
            if (API.getEntitySyncedData(player.handle, "Rota") == 0)
            {
                API.sendNotificationToPlayer(player, "~r~Você não está em uma rota.");
                return;
            }
            API.sendNotificationToPlayer(player, "~r~Você cancelou a rota.");

            API.triggerClientEvent(player, "finalize_job_bus", "bus");
            API.triggerClientEvent(player, "update_bus_job", false, 0, 0);

            if (API.hasEntitySyncedData(player.handle, "CarJob"))
            {
                NetHandle CarDel = (NetHandle)API.getEntitySyncedData(player.handle, "CarJob");
                API.deleteEntity(CarDel);
                API.setEntitySyncedData(player.handle, "CarJob", null);
            }

            API.setEntitySyncedData(player.handle, "Rota", 0);
            API.setEntitySyncedData(player.handle, "Parte", 0);
        }
    }

    public Vector3 GetNextPoint_LSS(Client player, int route, int nextpoint)
    {
        switch (route)
        {
            case 1: return Manut_1[nextpoint]; break;
            case 2: return Manut_2[nextpoint]; break;
            case 3: return Manut_3[nextpoint]; break;
        }
        return new Vector3();
    }
    public int GetTamanhoRota_LSS(int rota)
    {
        int Tamanho = 0;
        switch (rota)
        {
            case 1: Tamanho = Obras_1.Count; break;
            case 2: Tamanho = Obras_2.Count; break;
            case 3: Tamanho = Obras_3.Count; break;
        }
        return Tamanho;
    }
    #endregion

    [Command("usar")]
    public void CMD_usar(Client player, int opc)
    {
        if (!API.hasEntitySyncedData(player, "garrafa")) API.setEntitySyncedData(player, "garrafa", false);

        API.setEntitySyncedData(player, "garrafa", !API.getEntitySyncedData(player, "garrafa"));

        if (API.getEntitySyncedData(player, "garrafa"))
        {
            switch (opc)
            {
                case 1:
                    var garrafa = API.createObject(683570518, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa, player, "SKEL_R_Hand", new Vector3(0.12, 0.05, 0), new Vector3(-60, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa);
                    break;
                case 2: //Garrafa s/ papel
                    var garrafa1 = API.createObject(-440787091, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa1, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa1);
                    break;
                case 3: //Garrafa c/ papel
                    var garrafa2 = API.createObject(-1217150239, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa2, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa2);
                    break;
                case 4: //Garrafa de cerveja
                    var garrafa3 = API.createObject(683570518, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa3, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa3);
                    break;
                case 5: //Cigarro
                    var garrafa4 = API.createObject(2017086435, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa4, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa4);
                    break;
                case 6: //Lata de Spray
                    var garrafa5 = API.createObject(1749718958, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa5, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa5);
                    break;
                case 7: //Celular
                    var garrafa6 = API.createObject(-1038739674, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa6, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa6);
                    break;
                case 8: //Megafone
                    var garrafa7 = API.createObject(-1585551192, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa7, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa7);
                    break;
                case 9: //Baseado
                    var garrafa8 = API.createObject(-1199910959, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa8, player, "PH_L_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa8);
                    break;
                case 10: //Charuto fino
                    var garrafa9 = API.createObject(-110986183, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa9, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa9);
                    break;
                case 11: //Charuto grosso
                    var garrafa10 = API.createObject(-461945070, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa10, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa10);
                    break;
                case 12: //Pipe de metanfetamina
                    var garrafa11 = API.createObject(-212446848, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa11, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa11);
                    break;
                case 13: //Pipe de Crack
                    var garrafa12 = API.createObject(-693475324, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa12, player, "PH_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa12);
                    break;
                case 14: //Algemas
                    var garrafa13 = API.createObject(-1281059971, new Vector3(), new Vector3(), API.getEntityDimension(player));
                    API.attachEntityToEntity(garrafa13, player, "SKEL_R_Hand", new Vector3(0.0, 0.0, 0), new Vector3(0, 0, 0));
                    API.setEntitySyncedData(player, "garrafaobj", garrafa13);
                    break;

            } 

        }
        else
        {
            API.deleteEntity(API.getEntitySyncedData(player, "garrafaobj"));
        }

    }

    [Command("cenario")]
    public void CMD_cenario(Client player, int opc)
    {
        switch (opc)
        {
            case 1: API.playPlayerScenario(player, "WORLD_HUMAN_AA_COFFEE"); break;
            case 2: API.playPlayerScenario(player, "WORLD_HUMAN_AA_SMOKE"); break;
            case 3: API.playPlayerScenario(player, "WORLD_HUMAN_BINOCULARS"); break;
            case 4: API.playPlayerScenario(player, "WORLD_HUMAN_BUM_FREEWAY"); break;
            case 5: API.playPlayerScenario(player, "WORLD_HUMAN_BUM_SLUMPED"); break;
            case 6: API.playPlayerScenario(player, "WORLD_HUMAN_BUM_STANDING"); break;
            case 7: API.playPlayerScenario(player, "WORLD_HUMAN_BUM_WASH"); break;
            case 8: API.playPlayerScenario(player, "WORLD_HUMAN_CAR_PARK_ATTENDANT"); break;

            case 9: API.playPlayerScenario(player, "WORLD_HUMAN_CHEERING"); break;
            case 10:
                API.playPlayerScenario(player, "WORLD_HUMAN_CLIPBOARD"); break;
            case 11:
                API.playPlayerScenario(player, "WORLD_HUMAN_CONST_DRILL"); break; //Emprego Manutenção
            case 12:
                API.playPlayerScenario(player, "WORLD_HUMAN_COP_IDLES"); break;
            case 13:
                API.playPlayerScenario(player, "WORLD_HUMAN_DRINKING"); break;
            case 14:
                API.playPlayerScenario(player, "WORLD_HUMAN_DRUG_DEALER"); break;
            case 15:
                API.playPlayerScenario(player, "WORLD_HUMAN_DRUG_DEALER_HARD"); break;
            case 16:
                API.playPlayerScenario(player, "WORLD_HUMAN_MOBILE_FILM_SHOCKING"); break;
            case 17:
                API.playPlayerScenario(player, "WORLD_HUMAN_GARDENER_LEAF_BLOWER"); break; //Soprando plantas
            case 18:
                API.playPlayerScenario(player, "WORLD_HUMAN_GARDENER_PLANT"); break; //Jardineiro no chão
            case 19:
                API.playPlayerScenario(player, "WORLD_HUMAN_GOLF_PLAYER"); break;
            case 20:
                API.playPlayerScenario(player, "WORLD_HUMAN_GUARD_PATROL"); break;
            case 21:
                API.playPlayerScenario(player, "WORLD_HUMAN_GUARD_STAND"); break;
            case 22:
                API.playPlayerScenario(player, "WORLD_HUMAN_GUARD_STAND_ARMY"); break;
            case 23:
                API.playPlayerScenario(player, "WORLD_HUMAN_HAMMERING"); break; //Martelando - Emprego Manutenção
            case 24:
                API.playPlayerScenario(player, "WORLD_HUMAN_HANG_OUT_STREET"); break;
            case 25:
                API.playPlayerScenario(player, "WORLD_HUMAN_HIKER_STANDING"); break;
            case 26:
                API.playPlayerScenario(player, "WORLD_HUMAN_HUMAN_STATUE"); break;
            case 27:
                API.playPlayerScenario(player, "WORLD_HUMAN_JANITOR"); break;
            case 28:
                API.playPlayerScenario(player, "WORLD_HUMAN_JOG_STANDING"); break;
            case 29:
                API.playPlayerScenario(player, "WORLD_HUMAN_LEANING"); break;
            case 30:
                API.playPlayerScenario(player, "WORLD_HUMAN_MAID_CLEAN"); break;
            case 31:
                API.playPlayerScenario(player, "WORLD_HUMAN_MUSCLE_FLEX"); break;
            case 32:
                API.playPlayerScenario(player, "WORLD_HUMAN_MUSCLE_FREE_WEIGHTS"); break; //Levantando peso
            case 33:
                API.playPlayerScenario(player, "WORLD_HUMAN_MUSICIAN"); break; //Tocando violão
            case 34:
                API.playPlayerScenario(player, "WORLD_HUMAN_PAPARAZZI"); break; //Camera Fotografica
            case 35:
                API.playPlayerScenario(player, "WORLD_HUMAN_PARTYING"); break;
            case 36:
                API.playPlayerScenario(player, "WORLD_HUMAN_PICNIC"); break; //Sentado no chão
            case 37:
                API.playPlayerScenario(player, "WORLD_HUMAN_PROSTITUTE_HIGH_CLASS"); break;
            case 38:
                API.playPlayerScenario(player, "WORLD_HUMAN_PROSTITUTE_LOW_CLASS"); break;
            case 39:
                API.playPlayerScenario(player, "WORLD_HUMAN_PUSH_UPS"); break; //Flexão
            case 40:
                API.playPlayerScenario(player, "WORLD_HUMAN_SEAT_LEDGE"); break;
            case 41:
                API.playPlayerScenario(player, "WORLD_HUMAN_SEAT_LEDGE_EATING"); break;
            case 42:
                API.playPlayerScenario(player, "WORLD_HUMAN_SEAT_STEPS"); break;
            case 43:
                API.playPlayerScenario(player, "WORLD_HUMAN_SEAT_WALL"); break;
            case 44:
                API.playPlayerScenario(player, "WORLD_HUMAN_SEAT_WALL_EATING"); break;
            case 45:
                API.playPlayerScenario(player, "WORLD_HUMAN_SEAT_WALL_TABLET"); break;
            case 46:
                API.playPlayerScenario(player, "WORLD_HUMAN_SECURITY_SHINE_TORCH"); break;
            case 47:
                API.playPlayerScenario(player, "WORLD_HUMAN_SIT_UPS"); break; //Abdominal
            case 48:
                API.playPlayerScenario(player, "WORLD_HUMAN_SMOKING"); break; //Fumando parado
            case 49:
                API.playPlayerScenario(player, "WORLD_HUMAN_SMOKING_POT"); break;
            case 50:
                API.playPlayerScenario(player, "WORLD_HUMAN_STAND_FIRE"); break;
            case 51:
                API.playPlayerScenario(player, "WORLD_HUMAN_STAND_FISHING"); break; //Pescando - Com vara
            case 52:
                API.playPlayerScenario(player, "WORLD_HUMAN_STAND_IMPATIENT"); break;
            case 53:
                API.playPlayerScenario(player, "WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT"); break;
            case 54:
                API.playPlayerScenario(player, "WORLD_HUMAN_STAND_MOBILE"); break; //Parado mexendo no celular
            case 55:
                API.playPlayerScenario(player, "WORLD_HUMAN_STAND_MOBILE_UPRIGHT"); break;
            case 56:
                API.playPlayerScenario(player, "WORLD_HUMAN_STRIP_WATCH_STAND"); break; // Dançando suave
            case 57:
                API.playPlayerScenario(player, "WORLD_HUMAN_STUPOR"); break; //sentado e encostado
            case 58:
                API.playPlayerScenario(player, "WORLD_HUMAN_SUNBATHE"); break;//Deitado de bruço
            case 59:
                API.playPlayerScenario(player, "WORLD_HUMAN_SUNBATHE_BACK"); break;//Deitado suave
            case 60:
                API.playPlayerScenario(player, "WORLD_HUMAN_SUPERHERO"); break;
            case 61:
                API.playPlayerScenario(player, "WORLD_HUMAN_SWIMMING"); break;
            case 62:
                API.playPlayerScenario(player, "WORLD_HUMAN_TENNIS_PLAYER"); break; //Segurando raquete de tennis
            case 63:
                API.playPlayerScenario(player, "WORLD_HUMAN_TOURIST_MAP"); break; //Segurando mapa
            case 64:
                API.playPlayerScenario(player, "WORLD_HUMAN_TOURIST_MOBILE"); break; //
            case 65:
                API.playPlayerScenario(player, "WORLD_HUMAN_VEHICLE_MECHANIC"); break; // Em baixo do veiculo mexendo
            case 66:
                API.playPlayerScenario(player, "WORLD_HUMAN_WELDING"); break; //Usando Maçarico - Emprego Manutenção
            case 67:
                API.playPlayerScenario(player, "WORLD_HUMAN_WINDOW_SHOP_BROWSE"); break;
            case 68:
                API.playPlayerScenario(player, "WORLD_HUMAN_YOGA"); break; //Fazendo Yoga
            case 69:
                API.playPlayerScenario(player, "WORLD_BOAR_GRAZING"); break;
            case 70:
                API.playPlayerScenario(player, "WORLD_CAT_SLEEPING_GROUND"); break;
            case 71:
                API.playPlayerScenario(player, "WORLD_CAT_SLEEPING_LEDGE"); break;
            case 72:
                API.playPlayerScenario(player, "WORLD_COW_GRAZING"); break;
            case 73:
                API.playPlayerScenario(player, "WORLD_COYOTE_HOWL"); break;
            case 74:
                API.playPlayerScenario(player, "WORLD_COYOTE_REST"); break;
            case 75:
                API.playPlayerScenario(player, "WORLD_COYOTE_WANDER"); break;
            case 76:
                API.playPlayerScenario(player, "WORLD_CHICKENHAWK_FEEDING"); break;
            case 77:
                API.playPlayerScenario(player, "WORLD_CHICKENHAWK_STANDING"); break;
            case 78:
                API.playPlayerScenario(player, "WORLD_CORMORANT_STANDING"); break;
            case 79:
                API.playPlayerScenario(player, "WORLD_CROW_FEEDING"); break;
            case 80:
                API.playPlayerScenario(player, "WORLD_CROW_STANDING"); break;
            case 81:
                API.playPlayerScenario(player, "WORLD_DEER_GRAZING"); break;
            case 82:
                API.playPlayerScenario(player, "WORLD_DOG_BARKING_ROTTWEILER"); break;
            case 83:
                API.playPlayerScenario(player, "WORLD_DOG_BARKING_RETRIEVER"); break;
            case 84:
                API.playPlayerScenario(player, "WORLD_DOG_BARKING_SHEPHERD"); break;
            case 85:
                API.playPlayerScenario(player, "WORLD_DOG_SITTING_ROTTWEILER"); break;
            case 86:
                API.playPlayerScenario(player, "WORLD_DOG_SITTING_RETRIEVER"); break;
            case 87:
                API.playPlayerScenario(player, "WORLD_DOG_SITTING_SHEPHERD"); break;
            case 88:
                API.playPlayerScenario(player, "WORLD_DOG_BARKING_SMALL"); break;
            case 89:
                API.playPlayerScenario(player, "WORLD_DOG_SITTING_SMALL"); break;
            case 90:
                API.playPlayerScenario(player, "WORLD_FISH_IDLE"); break;
            case 100:
                API.playPlayerScenario(player, "WORLD_GULL_FEEDING"); break;
            case 101:
                API.playPlayerScenario(player, "WORLD_GULL_STANDING"); break;
            case 102:
                API.playPlayerScenario(player, "WORLD_HEN_PECKING"); break;
            case 103:
                API.playPlayerScenario(player, "WORLD_HEN_STANDING"); break;
            case 104:
                API.playPlayerScenario(player, "WORLD_MOUNTAIN_LION_REST"); break;
            case 105:
                API.playPlayerScenario(player, "WORLD_MOUNTAIN_LION_WANDER"); break;
            case 106:
                API.playPlayerScenario(player, "WORLD_PIG_GRAZING"); break;
            case 107:
                API.playPlayerScenario(player, "WORLD_PIGEON_FEEDING"); break;
            case 108:
                API.playPlayerScenario(player, "WORLD_PIGEON_STANDING"); break;
            case 109:
                API.playPlayerScenario(player, "WORLD_RABBIT_EATING"); break;
            case 110:
                API.playPlayerScenario(player, "WORLD_RATS_EATING"); break;
            case 111:
                API.playPlayerScenario(player, "WORLD_SHARK_SWIM"); break;
            case 112:
                API.playPlayerScenario(player, "PROP_BIRD_IN_TREE"); break;
            case 113:
                API.playPlayerScenario(player, "PROP_BIRD_TELEGRAPH_POLE"); break;
            case 114:
                API.playPlayerScenario(player, "PROP_HUMAN_ATM"); break; // Usando ATM
            case 115:
                API.playPlayerScenario(player, "PROP_HUMAN_BBQ"); break; // Fazendo Churrasco
            case 116:
                API.playPlayerScenario(player, "PROP_HUMAN_BUM_BIN"); break;
            case 117:
                API.playPlayerScenario(player, "PROP_HUMAN_BUM_SHOPPING_CART"); break; // Apioado no balcão
            case 118:
                API.playPlayerScenario(player, "PROP_HUMAN_MUSCLE_CHIN_UPS"); break; //Fazendo barras
            case 119:
                API.playPlayerScenario(player, "PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY"); break; //Fazendo barras 2
            case 120:
                API.playPlayerScenario(player, "PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON"); break; //Fazendo Barras 3
            case 121:
                API.playPlayerScenario(player, "PROP_HUMAN_PARKING_METER"); break;
            case 122:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_ARMCHAIR"); break;
            case 123:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_BAR"); break;
            case 124:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_BENCH"); break;
            case 125:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_BENCH_DRINK"); break;
            case 126:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_BENCH_DRINK_BEER"); break;
            case 127:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_BENCH_FOOD"); break;
            case 128:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_BUS_STOP_WAIT"); break;
            case 129:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_CHAIR"); break;
            case 130:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_CHAIR_DRINK"); break;
            case 131:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_CHAIR_DRINK_BEER"); break;
            case 132:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_CHAIR_FOOD"); break;
            case 134:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_CHAIR_UPRIGHT"); break;
            case 135:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_CHAIR_MP_PLAYER"); break;
            case 136:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_COMPUTER"); break;
            case 137:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_DECKCHAIR"); break;
            case 138:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_DECKCHAIR_DRINK"); break;
            case 139:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS"); break;
            case 140:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS_PRISON"); break;
            case 141:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_SEWING"); break;
            case 142:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_STRIP_WATCH"); break;
            case 143:
                API.playPlayerScenario(player, "PROP_HUMAN_SEAT_SUNLOUNGER"); break;
            case 144:
                API.playPlayerScenario(player, "PROP_HUMAN_STAND_IMPATIENT"); break;
            case 145:
                API.playPlayerScenario(player, "CODE_HUMAN_COWER"); break;
            case 146:
                API.playPlayerScenario(player, "CODE_HUMAN_CROSS_ROAD_WAIT"); break;
            case 147:
                API.playPlayerScenario(player, "CODE_HUMAN_PARK_CAR"); break;
            case 148:
                API.playPlayerScenario(player, "PROP_HUMAN_MOVIE_BULB"); break;
            case 149:
                API.playPlayerScenario(player, "PROP_HUMAN_MOVIE_STUDIO_LIGHT"); break;
            case 150:
                API.playPlayerScenario(player, "CODE_HUMAN_MEDIC_KNEEL"); break; //agaixado olhando
            case 151:
                API.playPlayerScenario(player, "CODE_HUMAN_MEDIC_TEND_TO_DEAD"); break;
            case 152:
                API.playPlayerScenario(player, "CODE_HUMAN_MEDIC_TIME_OF_DEATH"); break; //Anotando algo no caderno
            case 153:
                API.playPlayerScenario(player, "CODE_HUMAN_POLICE_CROWD_CONTROL"); break;
            case 154:
                API.playPlayerScenario(player, "CODE_HUMAN_POLICE_INVESTIGATE"); break;
            case 155:
                API.playPlayerScenario(player, "CODE_HUMAN_STAND_COWER"); break;
            case 156:
                API.playPlayerScenario(player, "EAR_TO_TEXT"); break;
            case 157:
                API.playPlayerScenario(player, "EAR_TO_TEXT_FAT"); break;
        }

    }

    #region Inventário

    [Command("i")]
    public void CMD_Inv1(Client player, string opc = "", int slot = 0)
    {
        CMD_Inv(player, opc, slot);
    }
    [Command("inv")]
    public void CMD_Inv(Client player, string opc = "", int slot = 0)
    {
        if (opc == "")
        {
            API.sendChatMessageToPlayer(player, "====== [Inventário de " + player.name + " ] ======");

            string line_1 = string.Format("[1:{0} ({1})] [2:{2} ({3})] [3:{4} ({5})] [4:{6} ({7})] [5:{8} ({9})] ", RetornarItemNomeBySlot(player, 1), RetornarItemQnt(player, 1), RetornarItemNomeBySlot(player, 2), RetornarItemQnt(player, 2), RetornarItemNomeBySlot(player, 3), RetornarItemQnt(player, 3), RetornarItemNomeBySlot(player, 4), RetornarItemQnt(player, 4), RetornarItemNomeBySlot(player, 5), RetornarItemQnt(player, 5));
            string line_2 = string.Format("[6:{0} ({1})] [7:{2} ({3})] [8:{4} ({5})] [9:{6} ({7})] [10:{8} ({9})] ", RetornarItemNomeBySlot(player, 6), RetornarItemQnt(player, 6), RetornarItemNomeBySlot(player, 7), RetornarItemQnt(player, 7), RetornarItemNomeBySlot(player, 8), RetornarItemQnt(player, 8), RetornarItemNomeBySlot(player, 9), RetornarItemQnt(player, 9), RetornarItemNomeBySlot(player, 10), RetornarItemQnt(player, 10));

            API.sendChatMessageToPlayer(player, line_1);
            API.sendChatMessageToPlayer(player, line_2);

            API.sendChatMessageToPlayer(player, "[USE]: /inv [opcao]");
            API.sendChatMessageToPlayer(player, "Opções: guardararma(ga), pegar(pe)");
            return;
        }

        int slot_vazio = 0;
        string inv_slot = string.Empty;
        string inv_slot_ammo = string.Empty;

        switch (opc)
        {
            case "ga":
            case "guardararma":

                if (API.getPlayerCurrentWeapon(player).ToString() == "Unarmed")
                {
                    EnviarMensagemErro(player, "Você não tem uma arma em mãos.");
                    return;
                }

                slot_vazio = EncontrarSlotVazio_Inv(player);
                if (slot_vazio == 99)
                {
                    EnviarMensagemErro(player, "Você não tem nenhum slot disponível. (Sem espaço)");
                    return;
                }

                inv_slot = string.Format("inv{0}", slot_vazio);
                inv_slot_ammo = string.Format("inv{0}q", slot_vazio);

                int ammo = API.getPlayerWeaponAmmo(player, API.getPlayerCurrentWeapon(player));

                switch (API.getPlayerCurrentWeapon(player).ToString())
                {
                    case "Machete":
                        ammo = 1;
                        break;
                }
                API.setEntitySyncedData(player, inv_slot, API.getPlayerCurrentWeapon(player).ToString());
                API.setEntitySyncedData(player, inv_slot_ammo, ammo);

                EnviarMensagemSucesso(player, "Você guardou um(a) " + API.getEntitySyncedData(player, inv_slot) + " com " + API.getEntitySyncedData(player, inv_slot_ammo) + " balas.");
                API.removePlayerWeapon(player, API.weaponNameToModel(API.getPlayerCurrentWeapon(player).ToString()));
                API.removeAllPlayerWeapons(player);
                break;
            case "pe":
            case "pegar":
                if (slot < 1 || slot > 10)
                {
                    EnviarMensagemErro(player, "Slot inválido, use de 1 a 10.");
                    return;
                }
                if (API.getPlayerCurrentWeapon(player).ToString() != "Unarmed")
                {
                    EnviarMensagemErro(player, "Você já tem uma arma em mãos.");
                    return;
                }
                inv_slot = string.Format("inv{0}", slot);
                inv_slot_ammo = string.Format("inv{0}q", slot);

                if (API.getEntitySyncedData(player, inv_slot) == "vazio")
                {
                    EnviarMensagemErro(player, "Não há nada neste Slot.");
                    return;
                }

                API.givePlayerWeapon(player, API.weaponNameToModel(API.getEntitySyncedData(player, inv_slot)), API.getEntitySyncedData(player, inv_slot_ammo), true, true);
                EnviarMensagemSucesso(player, "Você pegou um(a) " + API.getEntitySyncedData(player, inv_slot) + " com " + API.getEntitySyncedData(player, inv_slot_ammo) + " balas.");

                int return_w = RetornarHashArmaByName(API.getEntitySyncedData(player, inv_slot));
                if (return_w != 99)
                {
                    if (return_w != 0)
                    {
                        API.sendNativeToPlayer(player, Hash.SET_CURRENT_PED_WEAPON, player, return_w, 1);
                    }
                }
                else
                    EnviarMensagemErro(player, "deu ruim #0023");

                API.setEntitySyncedData(player, inv_slot, "vazio");
                API.setEntitySyncedData(player, inv_slot_ammo, 0);
                break;
            default:
                EnviarMensagemErro(player, "Opção inválida.");
                break;
        }
        SalvarPlayerInv(player);
    }
    [Command("resetarinv")]
    public void CMD_Resetinv(Client player, string playerid)
    {
        if (API.getEntitySyncedData(player, "staff") < 3)
        {
            EnviarMensagemErro(player, MSG_SEM_AUTORIZACAO);
            return;
        }

        var target = findPlayer(player, playerid);
        if (target == null)
        {
            EnviarMensagemErro(player, "Jogador não encontrado.");
            return;
        }

        ResetarPlayerInv(target);
    }

    public int EncontrarSlotVazio_Inv(Client player)
    {
        if (API.getEntitySyncedData(player, "inv1") == "vazio") return 1;
        if (API.getEntitySyncedData(player, "inv2") == "vazio") return 2;
        if (API.getEntitySyncedData(player, "inv3") == "vazio") return 3;
        if (API.getEntitySyncedData(player, "inv4") == "vazio") return 4;
        if (API.getEntitySyncedData(player, "inv5") == "vazio") return 5;
        if (API.getEntitySyncedData(player, "inv6") == "vazio") return 6;
        if (API.getEntitySyncedData(player, "inv7") == "vazio") return 7;
        if (API.getEntitySyncedData(player, "inv8") == "vazio") return 8;
        if (API.getEntitySyncedData(player, "inv9") == "vazio") return 9;
        if (API.getEntitySyncedData(player, "inv10") == "vazio") return 10;
        return 99;
    }

    public int RetornarItemQnt(Client player, int slot)
    {
        switch (slot)
        {
            case 1:
                return API.getEntitySyncedData(player, "inv1q");
            case 2:
                return API.getEntitySyncedData(player, "inv2q");
            case 3:
                return API.getEntitySyncedData(player, "inv3q");
            case 4:
                return API.getEntitySyncedData(player, "inv4q");
            case 5:
                return API.getEntitySyncedData(player, "inv5q");
            case 6:
                return API.getEntitySyncedData(player, "inv6q");
            case 7:
                return API.getEntitySyncedData(player, "inv7q");
            case 8:
                return API.getEntitySyncedData(player, "inv8q");
            case 9:
                return API.getEntitySyncedData(player, "inv9q");
            case 10:
                return API.getEntitySyncedData(player, "inv10q");
        }
        return 0;
    }

    public string RetornarItemNomeBySlot(Client player, int slot)
    {
        switch (slot)
        {
            case 1:
                return API.getEntitySyncedData(player, "inv1");
            case 2:
                return API.getEntitySyncedData(player, "inv2");
            case 3:
                return API.getEntitySyncedData(player, "inv3");
            case 4:
                return API.getEntitySyncedData(player, "inv4");
            case 5:
                return API.getEntitySyncedData(player, "inv5");
            case 6:
                return API.getEntitySyncedData(player, "inv6");
            case 7:
                return API.getEntitySyncedData(player, "inv7");
            case 8:
                return API.getEntitySyncedData(player, "inv8");
            case 9:
                return API.getEntitySyncedData(player, "inv9");
            case 10:
                return API.getEntitySyncedData(player, "inv10");
        }
        return "";
    }

    public void ResetarPlayerInv(Client player)
    {
        API.setEntitySyncedData(player, "inv1", "vazio");
        API.setEntitySyncedData(player, "inv2", "vazio");
        API.setEntitySyncedData(player, "inv3", "vazio");
        API.setEntitySyncedData(player, "inv4", "vazio");
        API.setEntitySyncedData(player, "inv5", "vazio");
        API.setEntitySyncedData(player, "inv6", "vazio");
        API.setEntitySyncedData(player, "inv7", "vazio");
        API.setEntitySyncedData(player, "inv8", "vazio");
        API.setEntitySyncedData(player, "inv9", "vazio");
        API.setEntitySyncedData(player, "inv10", "vazio");

        API.setEntitySyncedData(player, "inv1q", 0);
        API.setEntitySyncedData(player, "inv2q", 0);
        API.setEntitySyncedData(player, "inv3q", 0);
        API.setEntitySyncedData(player, "inv4q", 0);
        API.setEntitySyncedData(player, "inv5q", 0);
        API.setEntitySyncedData(player, "inv6q", 0);
        API.setEntitySyncedData(player, "inv7q", 0);
        API.setEntitySyncedData(player, "inv8q", 0);
        API.setEntitySyncedData(player, "inv9q", 0);
        API.setEntitySyncedData(player, "inv10q", 0);

        SalvarPlayerInv(player);
    }

    public string RetornarTipoDoItemByName(int tipo, string nome)
    {

        return "";
    }

    public string RetornarModelDoItemByName(int tipo, string nome)
    {

        return "";
    }

    public void SalvarPlayerInv(Client player)
    {
        var str = string.Format("UPDATE personagensinventario SET s1='{1}',s2='{2}',s3='{3}',s4='{4}',s5='{5}',s6='{6}',s7='{7}',s8='{8}',s9='{9}',s10='{10}',q1={11},q2={12},q3={13},q4={14},q5={15},q6={16},q7={17},q8={18},q9={19},q10={20} WHERE IDPersonagem={0}",
            API.getEntitySyncedData(player, "id"),
            API.getEntitySyncedData(player, "inv1"),
            API.getEntitySyncedData(player, "inv2"),
            API.getEntitySyncedData(player, "inv3"),
            API.getEntitySyncedData(player, "inv4"),
            API.getEntitySyncedData(player, "inv5"),
            API.getEntitySyncedData(player, "inv6"),
            API.getEntitySyncedData(player, "inv7"),
            API.getEntitySyncedData(player, "inv8"),
            API.getEntitySyncedData(player, "inv9"),
            API.getEntitySyncedData(player, "inv10"),
            API.getEntitySyncedData(player, "inv1q"),
            API.getEntitySyncedData(player, "inv2q"),
            API.getEntitySyncedData(player, "inv3q"),
            API.getEntitySyncedData(player, "inv4q"),
            API.getEntitySyncedData(player, "inv5q"),
            API.getEntitySyncedData(player, "inv6q"),
            API.getEntitySyncedData(player, "inv7q"),
            API.getEntitySyncedData(player, "inv8q"),
            API.getEntitySyncedData(player, "inv9q"),
            API.getEntitySyncedData(player, "inv10q")
            );
        var cmd = new MySqlCommand(str, bancodados);
        cmd.ExecuteNonQuery();

        return;
    }


    #endregion

    #region Central de Licenças & Licenças


    [Command("licencas")]
    public void cmd_licencas(Client player, String idOrName = "")
    {
        String temlic = "Não.";

        if (idOrName != "")
        {
            var target = findPlayer(player, idOrName);
            if (target == null)
            {
                API.sendChatMessageToPlayer(player, "Você mostrou suas licenças para " + target.nametagVisible + ".");

                API.sendChatMessageToPlayer(target, "- Licenças de " + player.nametagVisible + ".");
                //Motorista
                if (API.getEntitySyncedData(player, "carLic") == 1) temlic = "Sim.";
                API.sendChatMessageToPlayer(target, "Motorista: " + temlic);
            }
        }
        else
        {
            API.sendChatMessageToPlayer(player, "- Minhas licenças:");
            //Motorista
            if (API.getEntitySyncedData(player, "carLic") == 1) temlic = "Sim.";
            API.sendChatMessageToPlayer(player, "Motorista: " + temlic);
        }
    }
    //=======================================================================
    //                              AUTO ESCOLA
    //=======================================================================
    public ColShape AutoEscola_colShape;
    List<ColShape> Rota_AutoEscola = new List<ColShape>();
    List<Vector3> Caminho_AutoEscola = new List<Vector3>();

    public void CriarAutoEscola()
    {
        //Criando Rota 1
        Caminho_AutoEscola.Add(new Vector3(84.26405, -1683.661, 29.02724));//
        Caminho_AutoEscola.Add(new Vector3(162.4349, -1608.058, 29.1667));///
        Caminho_AutoEscola.Add(new Vector3(84.56377, -1518.733, 29.11484));//
        Caminho_AutoEscola.Add(new Vector3(142.2847, -1417.315, 29.12225));//
        Caminho_AutoEscola.Add(new Vector3(209.643, -1327.473, 29.16601));///
        Caminho_AutoEscola.Add(new Vector3(218.0594, -1145.901, 29.15763));//
        Caminho_AutoEscola.Add(new Vector3(218.8582, -1067.438, 29.05661));//
        Caminho_AutoEscola.Add(new Vector3(258.7469, -968.0418, 29.09279));//
        Caminho_AutoEscola.Add(new Vector3(385.9974, -957.0882, 29.21109));//
        Caminho_AutoEscola.Add(new Vector3(424.6748, -958.7374, 29.04519));//
        Caminho_AutoEscola.Add(new Vector3(485.1207, -956.8425, 27.18902));//
        Caminho_AutoEscola.Add(new Vector3(498.6256, -1115.417, 29.180849));//
        Caminho_AutoEscola.Add(new Vector3(498.2493, -1191.854, 29.0621));///
        Caminho_AutoEscola.Add(new Vector3(352.9987, -1307.172, 32.12413));//
        Caminho_AutoEscola.Add(new Vector3(304.8849, -1363.792, 31.76526));//
        Caminho_AutoEscola.Add(new Vector3(252.1195, -1426.917, 29.0915));//
        Caminho_AutoEscola.Add(new Vector3(178.1175, -1401.26, 29.1635));///
        Caminho_AutoEscola.Add(new Vector3(63.14182, -1281.713, 29.15909));//
        Caminho_AutoEscola.Add(new Vector3(66.93449, -1182.522, 29.16342));//
        Caminho_AutoEscola.Add(new Vector3(-80.41808, -1175.437, 37.14357));//
        Caminho_AutoEscola.Add(new Vector3(-284.2386, -1198.128, 36.99305));//
        Caminho_AutoEscola.Add(new Vector3(-424.2075, -1222.691, 45.69675));//
        Caminho_AutoEscola.Add(new Vector3(-457.6598, -1394.63, 29.34849));//
        Caminho_AutoEscola.Add(new Vector3(-392.0289, -1440.383, 29.08196));//
        Caminho_AutoEscola.Add(new Vector3(-231.649, -1451.124, 31.19447));//
        Caminho_AutoEscola.Add(new Vector3(-52.23782, -1600.996, 29.06773));//
        Caminho_AutoEscola.Add(new Vector3(153.2217, -1768.029, 28.78787));//
        Caminho_AutoEscola.Add(new Vector3(165.9049, -1750.722, 28.90826));//
        Caminho_AutoEscola.Add(new Vector3(116.1116, -1705.121, 28.97182));//

        for (int i = 0; i < Caminho_AutoEscola.Count; i++)
            Rota_AutoEscola.Add(API.createCylinderColShape(Caminho_AutoEscola[i], 4f, 3f));

        //Criando Icone, BLIP, TextLabel e Checkpoint na auto escola
        API.createMarker(0, new Vector3(138.8135, -1717.156, 29.29169), new Vector3(), new Vector3(), new Vector3(1f, 1f, 1f), 255, 255, 255, 0);
        Blip AutoEscola = API.createBlip(new Vector3(138.8135, -1717.156, 29.29169));
        API.setBlipSprite(AutoEscola, 498);
        API.setBlipColor(AutoEscola, 0);
        API.setBlipName(AutoEscola, "Centro de Licenças");
        API.setBlipShortRange(AutoEscola, true);
        AutoEscola_colShape = API.createCylinderColShape(new Vector3(138.8135, -1717.156, 29.29169), 2f, 3f);
        API.createTextLabel("Pressione Y para interagir com a Auto Escola.", new Vector3(138.8135, -1717.156, 29.29169), 8f, 0.6f);

        AutoEscola_colShape.onEntityExitColShape += (shape, entity) =>
        {
            
	            Client player;
	            if ((player = API.getPlayerFromHandle(entity)) != null)
	            {
	                API.triggerClientEvent(player, "dentro_colshape", 0);
	            }
        };
        AutoEscola_colShape.onEntityEnterColShape += (shape, entity) =>
        {
	            Client player;
	            if ((player = API.getPlayerFromHandle(entity)) != null)
	            {
	                API.triggerClientEvent(player, "dentro_colshape", 1);
	            }
        };
    }

    public Vector3 GetNextPoint_AutoEscola(Client player, int nextpoint)
    {
        return Caminho_AutoEscola[nextpoint];
    }
    public int GetTamanhoRota_AutoEscola()
    {
        return Rota_AutoEscola.Count;
    }
    #endregion

    [Command("coords")]
    public void coords(Client player, string coordName)
    {
        Vector3 playerPosGet = API.getEntityPosition(player);
        var pPosX = (playerPosGet.X.ToString().Replace(',', '.') + ", ");
        var pPosY = (playerPosGet.Y.ToString().Replace(',', '.') + ", ");
        var pPosZ = (playerPosGet.Z.ToString().Replace(',', '.'));
        Vector3 playerRotGet = API.getEntityRotation(player);
        var pRotX = (playerRotGet.X.ToString().Replace(',', '.') + ", ");
        var pRotY = (playerRotGet.Y.ToString().Replace(',', '.') + ", ");
        var pRotZ = (playerRotGet.Z.ToString().Replace(',', '.'));

        API.sendChatMessageToPlayer(player, "Your position is: ~y~" + playerPosGet, "~w~Your rotation is: ~y~" + playerRotGet);
        StreamWriter coordsFile;
        if (!File.Exists("SavedCoords.txt"))
        {
            coordsFile = new StreamWriter("SavedCoords.txt");
        }
        else
        {
            coordsFile = File.AppendText("SavedCoords.txt");
        }
        API.sendChatMessageToPlayer(player, "~r~Coordinates have been saved!");
        coordsFile.WriteLine("| " + coordName + " | " + "Saved Coordenates: " + pPosX + pPosY + pPosZ + " Saved Rotation: " + pRotX + pRotY + pRotZ);
        coordsFile.Close();
    }

    #region FuelSystem
    List<ColShape> PostoDeGasolina = new List<ColShape>();
    List<Vector3> PosicaoPostosDeGasolina = new List<Vector3>();

    public void CriarPostosDeGasolina()
    {
        //Criando Rota 1
        PosicaoPostosDeGasolina.Add(new Vector3(818.7, -1028.069, 26.40432));//0
        PosicaoPostosDeGasolina.Add(new Vector3(264.9478, -1260.342, 29.29144));//0
        PosicaoPostosDeGasolina.Add(new Vector3(174.6462, -1561.734, 29.26136));//0
        PosicaoPostosDeGasolina.Add(new Vector3(-319.5278, -1471.595, 30.5486));//0
        PosicaoPostosDeGasolina.Add(new Vector3(-525.2648, -1211.738, 18.18484));//0
        PosicaoPostosDeGasolina.Add(new Vector3(-723.2381, -936.222, 19.21384));//0
        PosicaoPostosDeGasolina.Add(new Vector3(-1436.93, -276.5609, 46.20768));//0
        PosicaoPostosDeGasolina.Add(new Vector3(-2096.708, -319.231, 13.16864));//0
        PosicaoPostosDeGasolina.Add(new Vector3(620.5403, 268.4994, 103.0895));//0
        PosicaoPostosDeGasolina.Add(new Vector3(1181.568, -330.2217, 69.31654));//0
        PosicaoPostosDeGasolina.Add(new Vector3(1208.219, -1402.626, 35.22414));//0

        for (int i = 0; i < PosicaoPostosDeGasolina.Count; i++)
            PostoDeGasolina.Add(API.createCylinderColShape(PosicaoPostosDeGasolina[i], 2f, 1f));
    }

    public float PlayerPertoPosto(Client player)
    {
        float retorno = 999.0f;
        float retornoFinal = 999.0f;

        for (int i = 0; i < PosicaoPostosDeGasolina.Count; i++)
        {
            retorno = player.position.DistanceTo(PosicaoPostosDeGasolina[i]);

            if (retorno < retornoFinal)
                retornoFinal = retorno;
        }
        return retornoFinal;
    }

    public void PlayerAbastecendo(Client player)
    {
        if (API.hasEntitySyncedData(player.handle, "player_abastecendo"))
        {
            if (API.isPlayerInAnyVehicle(player))
            {
                var veh = recuperarVeiculo(player.vehicle);
                if (veh == null) return;

                if (player.vehicle.engineStatus == true)
                {
                    EnviarMensagemErro(player, "Desligue o veículo para abastece-lo.");
                    API.resetEntitySyncedData(player.handle, "player_abastecendo");
                    return;
                }

                if ((veh.gasolina + 0.5f) < retornarMaxGasCar(veh.Modelo.ToString()))
                {
                    EnviarMensagemSucesso(player, "Abastecendo o seu " + veh.Modelo.ToString() + "\nPressione Y para parar.");
                    veh.gasolina = veh.gasolina + 0.5f;
                }
                else if ((veh.gasolina + 0.01f) < retornarMaxGasCar(veh.Modelo.ToString()))
                {
                    EnviarMensagemSucesso(player, "Abastecendo o seu " + veh.Modelo.ToString() + "\nPressione Y para parar.");
                    veh.gasolina = veh.gasolina + 0.01f;
                }
                else
                {
                    API.resetEntitySyncedData(player.handle, "player_abastecendo");
                    EnviarMensagemSucesso(player, "O tanque do seu " + veh.Modelo.ToString() + " está cheio.\nO abastecimento parou.");
                }
            }
        }
    }

    public void VerificarGasolinaVeiculos()
    {

        foreach (var veh in veiculos)
        {
            if (veh == null) continue;

            if (veh.Veh.engineStatus == true)
            {
                if (veh.gasolina <= 0.009f)
                {
                    veh.Veh.engineStatus = false;
                    veh.gasolina = 0;
                    foreach (var ocupante in veh.Veh.occupants)
                        if (API.getPlayerVehicleSeat(ocupante) == -1)
                            EnviarMensagemErro(ocupante, "O veículo ficou sem gasolina.");

                }
                if (veh.gasolina > retornarMaxGasCar(veh.Modelo.ToString()))
                    veh.gasolina = retornarMaxGasCar(veh.Modelo.ToString());


                var velocity = API.getEntityVelocity(veh.Veh);
                var speed = Math.Sqrt(
                        velocity.X * velocity.X +
                        velocity.Y * velocity.Y +
                        velocity.Z * velocity.Z
                        );

                var kmh = Math.Round(speed * 3.6);
                if (kmh > 95)
                    veh.gasolina = veh.gasolina - 0.006f;
                else
                    veh.gasolina = veh.gasolina - 0.002f;
            }
        }
    }

    public float retornarMaxGasCar(String vehmodel)
    {
        float litros = 99.9f;
        switch (vehmodel)
        {
            // Boats
            case "Dinghy": litros = 28.0f; break;
            case "Dinghy2": litros = 28.0f; break;
            case "Dinghy3": litros = 28.0f; break;
            case "Dinghy4": litros = 28.0f; break;
            case "Jetmax": litros = 45.0f; break;
            case "Marquis": litros = 50.0f; break;
            case "Seashark": litros = 12.0f; break;
            case "Seashark2": litros = 12.0f; break;
            case "Seashark3": litros = 12.0f; break;
            case "Speeder": litros = 40.0f; break;
            case "Speeder2": litros = 45.0f; break;
            case "Squalo": litros = 38.0f; break;
            case "Submersible": litros = 20.0f; break;
            case "Submersible2": litros = 12.0f; break;
            case "Suntrap": litros = 12.0f; break;
            case "Toro": litros = 43.0f; break;
            case "Toro2": litros = 45.0f; break;
            case "Tropic": litros = 22.0f; break;
            case "Tropic2": litros = 22.0f; break;
            case "Tug": litros = 80.0f; break;

            // Commercials
            case "Benson": litros = 48.0f; break;
            case "Biff": litros = 65.0f; break;
            case "Hauler": litros = 70.0f; break;
            case "Mule": litros = 55.0f; break;
            case "Mule2": litros = 55.0f; break;
            case "Mule3": litros = 55.0f; break;
            case "Packer": litros = 80.0f; break;
            case "Phantom": litros = 85.0f; break;
            case "Phantom2": litros = 85.0f; break;
            case "Pounder": litros = 55.0f; break;
            case "Stockade": litros = 50.0f; break;
            case "Stockade3": litros = 50.0f; break;

            // Compacts
            case "Blista": litros = 42.0f; break;
            case "Blista2": litros = 45.0f; break;
            case "Blista3": litros = 48.0f; break;
            case "Brioso": litros = 38.0f; break;
            case "Dilettante": litros = 45.0f; break;
            case "Dilettante2": litros = 45.0f; break;
            case "Issi2": litros = 38.0f; break;
            case "Panto": litros = 36.0f; break;
            case "Prairie": litros = 48.0f; break;
            case "Rhapsody": litros = 42.0f; break;

            // Coupes
            case "CogCabrio": litros = 48.0f; break;
            case "Exemplar": litros = 48.0f; break;
            case "F620": litros = 46.0f; break;
            case "Felon": litros = 48.0f; break;
            case "Felon2": litros = 48.0f; break;
            case "Jackal": litros = 50.0f; break;
            case "Oracle": litros = 48.0f; break;
            case "Oracle2": litros = 52.0f; break;
            case "Sentinel": litros = 38.0f; break;
            case "Sentinel2": litros = 38.0f; break;
            case "Windsor": litros = 40.0f; break;
            case "Windsor2": litros = 40.0f; break;
            case "Zion": litros = 46.0f; break;
            case "Zion2": litros = 46.0f; break;

            // Cycles
            case "Bmx": litros = 0.0f; break;
            case "Cruiser": litros = 0.0f; break;
            case "Fixter": litros = 0.0f; break;
            case "Scorcher": litros = 0.0f; break;
            case "TriBike": litros = 0.0f; break;
            case "TriBike2": litros = 0.0f; break;
            case "TriBike3": litros = 0.0f; break;

            // Emergency
            case "Ambulance": litros = 55.0f; break;
            case "FBI": litros = 48.0f; break;
            case "FBI2": litros = 62.0f; break;
            case "FireTruck": litros = 65.0f; break;
            case "PBus": litros = 70.0f; break;
            case "Police": litros = 48.0f; break;
            case "Police2": litros = 55.0f; break;
            case "Police3": litros = 48.0f; break;
            case "Police4": litros = 48.0f; break;
            case "PoliceOld1": litros = 50.0f; break;
            case "PoliceOld2": litros = 46.0f; break;
            case "PoliceT": litros = 55.0f; break;
            case "Policeb": litros = 20.0f; break;
            case "Polmav": litros = 28.0f; break;
            case "Pranger": litros = 62.0f; break;
            case "Predator": litros = 28.0f; break;
            case "Riot": litros = 50.0f; break;
            case "Sheriff": litros = 48.0f; break;
            case "Sheriff2": litros = 62.0f; break;

            // Helicopters
            case "Annihilator": litros = .0f; break;
            case "Buzzard": litros = 26.0f; break;
            case "Buzzard2": litros = 26.0f; break;
            case "Cargobob": litros = 60.0f; break;
            case "Cargobob2": litros = 60.0f; break;
            case "Cargobob3": litros = 60.0f; break;
            case "Cargobob4": litros = 60.0f; break;
            case "Frogger": litros = 32.0f; break;
            case "Frogger2": litros = 32.0f; break;
            case "Maverick": litros = 32.0f; break;
            case "Savage": litros = 26.0f; break;
            case "Skylift": litros = 32.0f; break;
            case "Supervolito": litros = 32.0f; break;
            case "Supervolito2": litros = 32.0f; break;
            case "Swift": litros = 30.0f; break;
            case "Swift2": litros = 30.0f; break;
            case "Valkyrie": litros = 36.0f; break;
            case "Valkyrie2": litros = 36.0f; break;
            case "Volatus": litros = 34.0f; break;

            // Industrial
            case "Bulldozer": litros = 50.0f; break;
            case "Cutter": litros = 50.0f; break;
            case "Dump": litros = 80.0f; break;
            case "Flatbed": litros = 65.0f; break;
            case "Guardian": litros = 70.0f; break;
            case "Handler": litros = 38.0f; break;
            case "Mixer": litros = 65.0f; break;
            case "Mixer2": litros = 65.0f; break;
            case "Rubble": litros = 65.0f; break;
            case "TipTruck": litros = 65.0f; break;
            case "TipTruck2": litros = 65.0f; break;

            // Military
            case "Barracks": litros = 70.0f; break;
            case "Barracks2": litros = 70.0f; break;
            case "Barracks3": litros = 70.0f; break;
            case "Crusader": litros = 55.0f; break;
            case "Rhino": litros = 65.0f; break;

            // Motorcycles
            case "Akuma": litros = 20.0f; break;
            case "Avarus": litros = 16.0f; break;
            case "Bagger": litros = 16.0f; break;
            case "Bati2": litros = 18.0f; break;
            case "Bati": litros = 18.0f; break;
            case "BF400": litros = 16.0f; break;
            case "Blazer4": litros = 14.0f; break;
            case "CarbonRS": litros = 18.0f; break;
            case "Chimera": litros = 18.0f; break;
            case "Cliffhanger": litros = 14.0f; break;
            case "Daemon2": litros = 18.0f; break;
            case "Daemon": litros = 18.0f; break;
            case "Defiler": litros = 20.0f; break;
            case "Double": litros = 18.0f; break;
            case "Enduro": litros = 16.0f; break;
            case "Esskey": litros = 20.0f; break;
            case "Faggio2": litros = 14.0f; break;
            case "Faggio3": litros = 14.0f; break;
            case "Faggio": litros = 14.0f; break;
            case "Fcr2": litros = 20.0f; break;
            case "Fcr": litros = 20.0f; break;
            case "Gargoyle": litros = 18.0f; break;
            case "Hakuchou2": litros = 18.0f; break;
            case "Hakuchou": litros = 18.0f; break;
            case "Hexer": litros = 18.0f; break;
            case "Innovation": litros = 16.0f; break;
            case "Lectro": litros = 16.0f; break;
            case "Manchez": litros = 16.0f; break;
            case "Nemesis": litros = 16.0f; break;
            case "Nightblade": litros = 18.0f; break;
            case "PCJ": litros = 18.0f; break;
            case "Ratbike": litros = 18.0f; break;
            case "Ruffian": litros = 20.0f; break;
            case "Sanchez2": litros = 16.0f; break;
            case "Sanchez": litros = 16.0f; break;
            case "Sanctus": litros = 18.0f; break;
            case "Shotaro": litros = 0.0f; break;
            case "Sovereign": litros = 20.0f; break;
            case "Thrust": litros = 20.0f; break;
            case "Vader": litros = 16.0f; break;
            case "Vindicator": litros = 20.0f; break;
            case "Vortex": litros = 20.0f; break;
            case "Wolfsbane": litros = 18.0f; break;
            case "Zombiea": litros = 18.0f; break;
            case "Zombieb": litros = 18.0f; break;

            // Muscle
            case "Blade": litros = 42.0f; break;
            case "Buccaneer": litros = 46.0f; break;
            case "Buccaneer2": litros = 46.0f; break;
            case "Chino": litros = 48.0f; break;
            case "Chino2": litros = 48.0f; break;
            case "Dominator": litros = 42.0f; break;
            case "Dominator2": litros = 42.0f; break;
            case "Dukes": litros = 46.0f; break;
            case "Dukes2": litros = 46.0f; break;
            case "Faction": litros = 42.0f; break;
            case "Faction2": litros = 42.0f; break;
            case "Faction3": litros = 42.0f; break;
            case "Gauntlet": litros = 42.0f; break;
            case "Gauntlet2": litros = 42.0f; break;
            case "Hotknife": litros = 36.0f; break;
            case "Lurcher": litros = 46.0f; break;
            case "Moonbeam": litros = 50.0f; break;
            case "Moonbeam2": litros = 50.0f; break;
            case "Nightshade": litros = 38.0f; break;
            case "Phoenix": litros = 42.0f; break;
            case "Picador": litros = 42.0f; break;
            case "RatLoader": litros = 46.0f; break;
            case "RatLoader2": litros = 46.0f; break;
            case "Ruiner": litros = 42.0f; break;
            case "Ruiner2": litros = 42.0f; break;
            case "SabreGT": litros = 42.0f; break;
            case "SabreGT2": litros = 42.0f; break;
            case "Sadler2": litros = 50.0f; break;
            case "SlamVan": litros = 46.0f; break;
            case "SlamVan2": litros = 46.0f; break;
            case "SlamVan3": litros = 46.0f; break;
            case "Stalion": litros = 42.0f; break;
            case "Stalion2": litros = 42.0f; break;
            case "Tampa": litros = 42.0f; break;
            case "Vigero": litros = 42.0f; break;
            case "Virgo": litros = 46.0f; break;
            case "Virgo2": litros = 46.0f; break;
            case "Virgo3": litros = 46.0f; break;
            case "Voodoo": litros = 46.0f; break;
            case "Voodoo2": litros = .0f; break;

            // Off-road
            case "BfInjection": litros = 42.0f; break;
            case "Bifta": litros = 38.0f; break;
            case "Blazer": litros = 24.0f; break;
            case "Blazer2": litros = 28.0f; break;
            case "Blazer3": litros = 24.0f; break;
            case "Blazer5": litros = 28.0f; break;
            case "Bodhi2": litros = 50.0f; break;
            case "Brawler": litros = 46.0f; break;
            case "DLoader": litros = 46.0f; break;
            case "Dune": litros = 28.0f; break;
            case "Dune2": litros = 28.0f; break;
            case "Dune4": litros = 28.0f; break;
            case "Dune5": litros = 28.0f; break;
            case "Insurgent": litros = 55.0f; break;
            case "Insurgent2": litros = 55.0f; break;
            case "Kalahari": litros = 48.0f; break;
            case "Lguard": litros = 55.0f; break;
            case "Marshall": litros = 50.0f; break;
            case "Mesa": litros = 50.0f; break;
            case "Mesa2": litros = 50.0f; break;
            case "Mesa3": litros = 50.0f; break;
            case "Monster": litros = 50.0f; break;
            case "RancherXL": litros = 55.0f; break;
            case "RancherXL2": litros = 55.0f; break;
            case "Rebel": litros = 48.0f; break;
            case "Rebel2": litros = 48.0f; break;
            case "Sandking": litros = 60.0f; break;
            case "Sandking2": litros = 60.0f; break;
            case "Technical": litros = 48.0f; break;
            case "Technical2": litros = 48.0f; break;
            case "TrophyTruck": litros = 38.0f; break;
            case "TrophyTruck2": litros = 38.0f; break;

            // Planes
            case "Besra": litros = 75.0f; break;
            case "Blimp": litros = 100.0f; break;
            case "Blimp2": litros = 100.0f; break;
            case "CargoPlane": litros = 300.0f; break;
            case "Cuban800": litros = 100.0f; break;
            case "Dodo": litros = 98.0f; break;
            case "Duster": litros = 86.0f; break;
            case "Hydra": litros = 72.0f; break;
            case "Jet": litros = 278.0f; break;
            case "Lazer": litros = 74.0f; break;
            case "Luxor": litros = 116.0f; break;
            case "Luxor2": litros = 116.0f; break;
            case "Mammatus": litros = 100.0f; break;
            case "Miljet": litros = 132.0f; break;
            case "Nimbus": litros = 108.0f; break;
            case "Shamal": litros = 100.0f; break;
            case "Stunt": litros = 68.0f; break;
            case "Titan": litros = 184.0f; break;
            case "Velum": litros = 112.0f; break;
            case "Velum2": litros = 112.0f; break;
            case "Vestra": litros = 74.0f; break;

            // SUVs
            case "BJXL": litros = 50.0f; break;
            case "Baller": litros = 50.0f; break;
            case "Baller2": litros = 52.0f; break;
            case "Baller3": litros = 52.0f; break;
            case "Baller4": litros = 52.0f; break;
            case "Baller5": litros = 52.0f; break;
            case "Baller6": litros = 52.0f; break;
            case "Cavalcade": litros = 56.0f; break;
            case "Cavalcade2": litros = 58.0f; break;
            case "Contender": litros = 64.0f; break;
            case "Dubsta": litros = 55.0f; break;
            case "Dubsta2": litros = 55.0f; break;
            case "Dubsta3": litros = 64.0f; break;
            case "FQ2": litros = 50.0f; break;
            case "Granger": litros = 55.0f; break;
            case "Gresley": litros = 50.0f; break;
            case "Habanero": litros = 48.0f; break;
            case "Huntley": litros = 55.0f; break;
            case "Landstalker": litros = 55.0f; break;
            case "Patriot": litros = 58.0f; break;
            case "Radi": litros = 46.0f; break;
            case "Rocoto": litros = 50.0f; break;
            case "Seminole": litros = 50.0f; break;
            case "Serrano": litros = 48.0f; break;
            case "XLS": litros = 52.0f; break;
            case "XLS2": litros = 52.0f; break;

            // Sedans
            case "Asea": litros = 42.0f; break;
            case "Asea2": litros = 42.0f; break;
            case "Asterope": litros = 46.0f; break;
            case "Cog55": litros = 42.0f; break;
            case "Cog552": litros = 42.0f; break;
            case "Cognoscenti": litros = 46.0f; break;
            case "Cognoscenti2": litros = 46.0f; break;
            case "Emperor": litros = 48.0f; break;
            case "Emperor2": litros = 48.0f; break;
            case "Emperor3": litros = 48.0f; break;
            case "Fugitive": litros = 50.0f; break;
            case "Glendale": litros = 46.0f; break;
            case "Ingot": litros = 44.0f; break;
            case "Intruder": litros = 46.0f; break;
            case "Limo2": litros = 50.0f; break;
            case "Premier": litros = 46.0f; break;
            case "Primo": litros = 48.0f; break;
            case "Primo2": litros = 48.0f; break;
            case "Regina": litros = 42.0f; break;
            case "Romero": litros = 48.0f; break;
            case "Stanier": litros = 48.0f; break;
            case "Stratum": litros = 48.0f; break;
            case "Stretch": litros = 50.0f; break;
            case "Surge": litros = 0.0f; break; //Elétrico
            case "Tailgater": litros = 48.0f; break;
            case "Warrener": litros = 42.0f; break;
            case "Washington": litros = 50.0f; break;

            // Service
            case "Airbus": litros = 84.0f; break;
            case "Brickade": litros = .0f; break;
            case "Bus": litros = 84.0f; break;
            case "Coach": litros = 88.0f; break;
            case "Rallytruck": litros = 72.0f; break;
            case "RentalBus": litros = 55.0f; break;
            case "Taxi": litros = 48.0f; break;
            case "Tourbus": litros = 55.0f; break;
            case "Trash": litros = 72.0f; break;
            case "Trash2": litros = 72.0f; break;

            // Sports
            case "Alpha": litros = 38.0f; break;
            case "Banshee": litros = 42.0f; break;
            case "Banshee2": litros = 42.0f; break;
            case "BestiaGTS": litros = 42.0f; break;
            case "Buffalo": litros = 48.0f; break;
            case "Buffalo2": litros = 48.0f; break;
            case "Buffalo3": litros = 48.0f; break;
            case "Carbonizzare": litros = 38.0f; break;
            case "Comet2": litros = 38.0f; break;
            case "Comet3": litros = 42.0f; break;
            case "Coquette": litros = 38.0f; break;
            case "Elegy": litros = 46.0f; break;
            case "Elegy2": litros = 42.0f; break;
            case "Feltzer2": litros = 38.0f; break;
            case "Feltzer3": litros = 36.0f; break;
            case "Furoregt": litros = 42.0f; break;
            case "Fusilade": litros = 38.0f; break;
            case "Futo": litros = 38.0f; break;
            case "Infernus2": litros = 42.0f; break;
            case "Jester": litros = 42.0f; break;
            case "Jester2": litros = 42.0f; break;
            case "Khamelion": litros = 0.0f; break; //Elétrico
            case "Kuruma": litros = 46.0f; break;
            case "Kuruma2": litros = 46.0f; break;
            case "Lynx": litros = 42.0f; break;
            case "Massacro": litros = 42.0f; break;
            case "Massacro2": litros = 42.0f; break;
            case "Ninef": litros = 38.0f; break;
            case "Ninef2": litros = 38.0f; break;
            case "Omnis": litros = 36.0f; break;
            case "Penumbra": litros = 42.0f; break;
            case "RapidGT": litros = 42.0f; break;
            case "RapidGT2": litros = 42.0f; break;
            case "Raptor": litros = 34.0f; break;
            case "Ruston": litros = 38.0f; break;
            case "Schafter2": litros = 46.0f; break;
            case "Schafter3": litros = 46.0f; break;
            case "Schafter4": litros = 46.0f; break;
            case "Schafter5": litros = 46.0f; break;
            case "Schafter6": litros = 46.0f; break;
            case "Schwarzer": litros = 42.0f; break;
            case "Seven70": litros = 38.0f; break;
            case "Specter": litros = 44.0f; break;
            case "Specter2": litros = 44.0f; break;
            case "Sultan": litros = 48.0f; break;
            case "Surano": litros = 42.0f; break;
            case "Tampa2": litros = 38.0f; break;
            case "Tropos": litros = 38.0f; break;
            case "Verlierer2": litros = 38.0f; break;

            // Sports Classic
            case "BType": litros = 36.0f; break;
            case "BType2": litros = 38.0f; break;
            case "BType3": litros = 36.0f; break;
            case "Casco": litros = 42.0f; break;
            case "Coquette2": litros = 38.0f; break;
            case "Coquette3": litros = 42.0f; break;
            case "JB700": litros = 42.0f; break;
            case "Mamba": litros = 38.0f; break;
            case "Manana": litros = 46.0f; break;
            case "Monroe": litros = 42.0f; break;
            case "Peyote": litros = 38.0f; break;
            case "Pigalle": litros = 36.0f; break;
            case "Stinger": litros = 38.0f; break;
            case "StingerGT": litros = 38.0f; break;
            case "Tornado": litros = 48.0f; break;
            case "Tornado2": litros = 48.0f; break;
            case "Tornado3": litros = 48.0f; break;
            case "Tornado4": litros = 48.0f; break;
            case "Tornado5": litros = 48.0f; break;
            case "Tornado6": litros = 38.0f; break;
            case "ZType": litros = 42.0f; break;

            // Super
            case "Adder": litros = 42.0f; break;
            case "Bullet": litros = 42.0f; break;
            case "Cheetah": litros = 38.0f; break;
            case "EntityXF": litros = 42.0f; break;
            case "FMJ": litros = 42.0f; break;
            case "GP1": litros = 42.0f; break;
            case "Infernus": litros = 46.0f; break;
            case "LE7B": litros = 38.0f; break;
            case "Nero": litros = 46.0f; break;
            case "Nero2": litros = 46.0f; break;
            case "Osiris": litros = 42.0f; break;
            case "Penetrator": litros = 42.0f; break;
            case "Pfister811": litros = 38.0f; break;
            case "Prototipo": litros = 36.0f; break;
            case "Reaper": litros = 42.0f; break;
            case "Sheava": litros = 42.0f; break;
            case "SultanRS": litros = 48.0f; break;
            case "Superd": litros = 50.0f; break;
            case "T20": litros = 38.0f; break;
            case "Tempesta": litros = 42.0f; break;
            case "Turismo2": litros = 42.0f; break;
            case "Turismor": litros = 42.0f; break;
            case "Tyrus": litros = 46.0f; break;
            case "Vacca": litros = 42.0f; break;
            case "Voltic": litros = 0.0f; break; //Elétrico
            case "Voltic2": litros = 42.0f; break;
            case "Zentorno": litros = 46.0f; break;
            case "Italigtb": litros = 42.0f; break;
            case "Italigtb2": litros = 42.0f; break;

            // Trailer
            case "ArmyTanker": litros = 0.0f; break;
            case "ArmyTrailer": litros = 0.0f; break;
            case "ArmyTrailer2": litros = 0.0f; break;
            case "BaleTrailer": litros = 0.0f; break;
            case "BoatTrailer": litros = 0.0f; break;
            case "CableCar": litros = 0.0f; break;
            case "DockTrailer": litros = 0.0f; break;
            case "GrainTrailer": litros = 0.0f; break;
            case "PropTrailer": litros = 0.0f; break;
            case "RakeTrailer": litros = 0.0f; break;
            case "TR2": litros = 0.0f; break;
            case "TR3": litros = 0.0f; break;
            case "TR4": litros = 0.0f; break;
            case "TRFlat": litros = 0.0f; break;
            case "TVTrailer": litros = 0.0f; break;
            case "Tanker": litros = 0.0f; break;
            case "Tanker2": litros = 0.0f; break;
            case "TrailerLogs": litros = 0.0f; break;
            case "TrailerSmall": litros = 0.0f; break;
            case "Trailers": litros = 0.0f; break;
            case "Trailers2": litros = 0.0f; break;
            case "Trailers3": litros = 0.0f; break;

            // Trains
            case "Freight": litros = 0.0f; break;
            case "FreightCar": litros = 0.0f; break;
            case "FreightCont1": litros = 0.0f; break;
            case "FreightCont2": litros = 0.0f; break;
            case "FreightGrain": litros = 0.0f; break;
            case "FreightTrailer": litros = 0.0f; break;
            case "TankerCar": litros = 0.0f; break;

            // Utility
            case "Airtug": litros = 24.0f; break;
            case "Caddy": litros = 22.0f; break;
            case "Caddy2": litros = 22.0f; break;
            case "Docktug": litros = 28.0f; break;
            case "Forklift": litros = 22.0f; break;
            case "Mower": litros = 16.0f; break;
            case "Ripley": litros = 50.0f; break;
            case "Sadler": litros = 50.0f; break;
            case "Scrap": litros = 70.0f; break;
            case "TowTruck": litros = 70.0f; break;
            case "TowTruck2": litros = 48.0f; break;
            case "Tractor": litros = 22.0f; break;
            case "Tractor2": litros = 34.0f; break;
            case "Tractor3": litros = 34.0f; break;
            case "UtilityTruck": litros = 55.0f; break;
            case "UtilityTruck3": litros = 55.0f; break;
            case "UtilliTruck2": litros = 55.0f; break;

            // Vans
            case "Bison": litros = 50.0f; break;
            case "Bison2": litros = 50.0f; break;
            case "Bison3": litros = 50.0f; break;
            case "BobcatXL": litros = 48.0f; break;
            case "Boxville": litros = 62.0f; break;
            case "Boxville2": litros = 62.0f; break;
            case "Boxville3": litros = 62.0f; break;
            case "Boxville4": litros = 62.0f; break;
            case "Boxville5": litros = 62.0f; break;
            case "Burrito": litros = 56.0f; break;
            case "Burrito2": litros = 56.0f; break;
            case "Burrito3": litros = 56.0f; break;
            case "Burrito4": litros = 56.0f; break;
            case "Burrito5": litros = 56.0f; break;
            case "Camper": litros = 72.0f; break;
            case "GBurrito": litros = 50.0f; break;
            case "GBurrito2": litros = 50.0f; break;
            case "Journey": litros = 64.0f; break;
            case "Minivan": litros = 46.0f; break;
            case "Minivan2": litros = 46.0f; break;
            case "Paradise": litros = 52.0f; break;
            case "Pony": litros = 52.0f; break;
            case "Pony2": litros = 52.0f; break;
            case "Rumpo": litros = 54.0f; break;
            case "Rumpo2": litros = 54.0f; break;
            case "Rumpo3": litros = 54.0f; break;
            case "Speedo": litros = 52.0f; break;
            case "Speedo2": litros = 52.0f; break;
            case "Surfer": litros = 58.0f; break;
            case "Surfer2": litros = 58.0f; break;
            case "Taco": litros = 64.0f; break;
            case "Youga": litros = 52.0f; break;
            case "Youga2": litros = 48.0f; break;
        }
        return litros;
    }
    #endregion
    public void SalvarVeiculos()
    {
        foreach (var veh in veiculos)
        {
            if(veh.Veh == null) continue;

            var strUpdate = string.Format("UPDATE veiculos SET PosX={0},PosZ={1},PosY={2},RotX={3},RotZ={4},RotY={5},gasolina={6} WHERE ID={7}",
                        aFlt(veh.Veh.position.X),
                        aFlt(veh.Veh.position.Z),
                        aFlt(veh.Veh.position.Y),
                        aFlt(veh.Veh.rotation.X),
                        aFlt(veh.Veh.rotation.Z),
                        aFlt(veh.Veh.rotation.Y),
                        aFlt(veh.gasolina),
                        veh.ID);
            var cmd = new MySqlCommand(strUpdate, bancodados);
            cmd.ExecuteNonQuery();

            veh.Veh.delete();
        }
        API.consoleOutput("[SAVE]: Veiculos salvos.");
        
    }

    [Command("rag")]
    public void CMD_ragdoll(Client player, int tipo)
    {
        if(tipo == 5)
            API.triggerClientEvent(player, "player_ragdoll_c");
        else
            API.triggerClientEvent(player, "player_ragdoll", tipo);
    }

    [Command("loadint")]
    public void CMD_morgue(Client player, int inte, int vari = 0)
    {
        switch (inte)
        {
            case 0:
                API.sendNotificationToAll("Joalheria Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "post_hiest_unload");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "jewel2fake");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "bh1_16_refurb");
                break;
            case 1:
                API.sendNotificationToAll("Morgue Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "Coroner_Int_on");
                API.setEntityPosition(player, new Vector3(244.9f, -1374.7f, 39.5f));
                break;
            case 2:
                API.sendNotificationToAll("cargoship Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "cargoship");
                API.setEntityPosition(player, new Vector3(-90.0f, -2365.8f, 14.3f));
                break;
            case 3:
                API.sendNotificationToAll("Heist Carrier Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_carrier");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_carrier_DistantLights");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_Carrier_int1");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_Carrier_int2");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_Carrier_int3");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_Carrier_int4");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_Carrier_int5");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_Carrier_int6");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "hei_carrier_LODLights");
                API.setEntityPosition(player, new Vector3(-3069.9f, -4632.4f, 16.2f));
                break;
            case 4:
                API.sendNotificationToAll("O'Neil Ranch Load");
                if (vari == 0)
                {
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm");
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm_props");
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farmint");
                }
                else if (vari == 1)
                {
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm");
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm_props");
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farmint");

                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm_burnt");
                }
                else if (vari == 2)
                {
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm");
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm_props");
                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farmint");

                    API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farm_burnt_props");
                }
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "farmint_cap");
                API.setEntityPosition(player, new Vector3(2441.2f, 4968.5f, 51.7f));
                break;
            case 5:
                API.sendNotificationToAll("LifeInvader Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "facelobbyfake");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "facelobby");
                API.setEntityPosition(player, new Vector3(-1047.9f, -233.0f, 39.0f));
                break;
            case 6:
                API.sendNotificationToAll("Cluckin bell Load");

                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "CS1_02_cf_offmission");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "CS1_02_cf_onmission1");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "CS1_02_cf_onmission2");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "CS1_02_cf_onmission3");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "CS1_02_cf_onmission4");
                API.setEntityPosition(player, new Vector3(-72.68752, 6253.72656, 31.08991));
                break;
            case 7:
                API.sendNotificationToAll("Train Crash Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "canyonriver01_traincrash");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "railing_end");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "railing_start");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "canyonriver01");
                API.setEntityPosition(player, new Vector3(-532.1309, 4526.187, 88.7955));
                break;
            case 8:
                API.sendNotificationToAll("Broken Stilt House Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "DES_StiltHouse_imapend");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "DES_StiltHouse_imapstart");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "des_stilthouse_rebuild");
                API.setEntityPosition(player, new Vector3(-1020.5, 663.41, 154.75));
                break;
            case 9:
                API.sendNotificationToAll("Plane Crash Load");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "Plane_crash_trench");
                API.setEntityPosition(player, new Vector3(2814.7, 4758.5, 50.0));
                break;
        }
    }
    [Command("unloadint")]
    public void CMD_morgueun(Client player, int inte)
    {
        switch (inte)
        {
            case 0:
                API.sendNotificationToAll("Joalheria unLoad");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "post_hiest_unload");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "jewel2fake");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "bh1_16_refurb");
                break;
            case 1:
                API.sendNotificationToAll("Morgue unLoad");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "Coroner_Int_on");
                API.setEntityPosition(player, new Vector3(244.9f, -1374.7f, 39.5f));
                break;
            case 2:
                API.sendNotificationToAll("cargoship unLoad");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "cargoship");
                API.setEntityPosition(player, new Vector3(-90.0f, -2365.8f, 14.3f));
                break;
            case 3:
                API.sendNotificationToAll("Heist Carrier unLoad");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_carrier");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_carrier_DistantLights");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_Carrier_int1");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_Carrier_int2");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_Carrier_int3");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_Carrier_int4");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_Carrier_int5");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_Carrier_int6");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "hei_carrier_LODLights");
                API.setEntityPosition(player, new Vector3(-3069.9f, -4632.4f, 16.2f));
                break;
            case 4:
                API.sendNotificationToAll("O'Neil Ranch Load");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "farm");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "farm_props");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "farmint");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "farm_burnt");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "farm_burnt_props");
                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "farmint_cap");
                API.setEntityPosition(player, new Vector3(2441.2f, 4968.5f, 51.7f));
                break;
            case 5:
                API.sendNotificationToAll("LifeInvader Load");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "facelobbyfake");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "facelobby");
                API.setEntityPosition(player, new Vector3(-1047.9f, -233.0f, 39.0f));
                break;
            case 6:
                API.sendNotificationToAll("Cluckin bell Load");

                API.sendNativeToAllPlayers(Hash.REQUEST_IPL, "CS1_02_cf_offmission");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "CS1_02_cf_onmission1");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "CS1_02_cf_onmission2");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "CS1_02_cf_onmission3");
                API.sendNativeToAllPlayers(Hash.REMOVE_IPL, "CS1_02_cf_onmission4");
                API.setEntityPosition(player, new Vector3(-72.68752, 6253.72656, 31.08991));
                break;
        }
    }
}
