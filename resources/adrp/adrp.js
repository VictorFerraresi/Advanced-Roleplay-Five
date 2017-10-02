/* 
http://five.advanced-roleplay.com.br/forum
|----------------------------------------------------------| Advanced Roleplay |----------------------------------------------------------|
First
TR3V1Z4
Freeze
*/

var res = API.getScreenResolution();
var resR = API.getScreenResolutionMaintainRatio();
var loginBrowser = null;
var banBrowser = null;
var helpBrowser = null;
var playersBrowser = null;
var statsBrowser = null;
var sosBrowser = null;
var atmBrowser = null;
var conceBrowser = null;
var creatercharBrowser = null;
var autoescolaBrowser = null;

var menuPool = null;

var param1 = null;
var param2 = null;

//Empregos
var pontoAtual = 0;
var PontosRestantes = 0;
var res_X = API.getScreenResolutionMaintainRatio().Width;
var res_Y = API.getScreenResolutionMaintainRatio().Height;

var InBusStop = false;
var garbagePickedUp = "Va para o próximo ponto";
var OnPickupJob = 0;
var PlayerInMarker = 0;

var Congelado = 0;
var VehCongelado;
var markers = {};

class JobHelper {

    static createMarker(name, position, radius) {
        if (markers[name] != null) { return; }

        var marker = API.createMarker(
            1,
            position,
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(radius, radius, radius),
            255, 0, 0, 100
        );

        markers[name] = marker;

        return marker;
    }

    static removeMarker(name) {
        if (markers[name] == null) { return; }

        API.deleteEntity(markers[name]);
        markers[name] = null;
    }

}

/* Menu Camisetas */
var menu_camisetas = API.createMenu("Binco", "Selecione uma camiseta!", 0, 0, 6);

menu_camisetas.AddItem(API.createMenuItem("Lisa", "Experimentar")); // 1
menu_camisetas.AddItem(API.createMenuItem("Regata", "Experimentar")); // 2
menu_camisetas.AddItem(API.createMenuItem("Manga longa", "Experimentar")); // 3
menu_camisetas.AddItem(API.createMenuItem("Polo", "Experimentar")); // 4
menu_camisetas.AddItem(API.createMenuItem("Social-Casual", "Experimentar")); // 5
menu_camisetas.AddItem(API.createMenuItem("Social", "Experimentar")); // 6
menu_camisetas.AddItem(API.createMenuItem("Xadrez", "Experimentar")); // 7
menu_camisetas.AddItem(API.createMenuItem("Lisa 2", "Experimentar")); // 8
menu_camisetas.AddItem(API.createMenuItem("Regata 2", "Experimentar")); // 9
menu_camisetas.AddItem(API.createMenuItem("Natalina", "Experimentar")); // 10
menu_camisetas.AddItem(API.createMenuItem("Lisa 3", "Experimentar")); // 11
menu_camisetas.AddItem(API.createMenuItem("Lisa 4", "Experimentar")); // 12
menu_camisetas.AddItem(API.createMenuItem("Social 2", "Experimentar")); // 13
menu_camisetas.AddItem(API.createMenuItem("Listrada", "Experimentar")); // 14
menu_camisetas.AddItem(API.createMenuItem("Lisa 5", "Experimentar")); // 15
menu_camisetas.AddItem(API.createMenuItem("Regata 3", "Experimentar")); // 16
menu_camisetas.AddItem(API.createMenuItem("Manga Longa 2", "Experimentar")); // 17
menu_camisetas.AddItem(API.createMenuItem("Polo 2", "Experimentar")); // 18
menu_camisetas.AddItem(API.createMenuItem("Xadez casual", "Experimentar")); // 19
menu_camisetas.AddItem(API.createMenuItem("Social com suspensorio", "Experimentar")); // 20
menu_camisetas.AddItem(API.createMenuItem("Social com suspensorio fechada", "Experimentar")); // 21
menu_camisetas.AddItem(API.createMenuItem("Lisa 6", "Experimentar")); // 22
menu_camisetas.AddItem(API.createMenuItem("Lisa 7", "Experimentar")); // 23
menu_camisetas.AddItem(API.createMenuItem("O Agente", "Experimentar")); // 26
menu_camisetas.AddItem(API.createMenuItem("Sujinha", "Experimentar")); // 28
menu_camisetas.AddItem(API.createMenuItem("Social-Casual", "Experimentar")); // 31
menu_camisetas.AddItem(API.createMenuItem("O louco", "Experimentar")); // 32
menu_camisetas.AddItem(API.createMenuItem("Dourada", "Experimentar")); // 34
menu_camisetas.AddItem(API.createMenuItem("Estampada", "Experimentar")); // 35
menu_camisetas.AddItem(API.createMenuItem("Largada", "Experimentar")); // 39
menu_camisetas.AddItem(API.createMenuItem("Largada 2", "Experimentar")); // 40
menu_camisetas.AddItem(API.createMenuItem("Polo comprida", "Experimentar")); // 41
menu_camisetas.AddItem(API.createMenuItem("Baseball", "Experimentar")); // 42
menu_camisetas.AddItem(API.createMenuItem("Social-Largado", "Experimentar")); // 44
menu_camisetas.AddItem(API.createMenuItem("Polo 3", "Experimentar")); // 49
menu_camisetas.AddItem(API.createMenuItem("Polo 3 (Para dentro)", "Experimentar")); // 50
menu_camisetas.AddItem(API.createMenuItem("Social meia manga", "Experimentar")); // 51
menu_camisetas.AddItem(API.createMenuItem("Lisa 8", "Experimentar")); // 53
menu_camisetas.AddItem(API.createMenuItem("Tatica (Manga longa)", "Experimentar")); // 54
menu_camisetas.AddItem(API.createMenuItem("Havaiana", "Experimentar")); // 55
menu_camisetas.AddItem(API.createMenuItem("Oriental Classica", "Experimentar")); // 56
menu_camisetas.AddItem(API.createMenuItem("Polo 4", "Experimentar")); // 60
menu_camisetas.AddItem(API.createMenuItem("Xadrez 2", "Experimentar")); // 62
menu_camisetas.AddItem(API.createMenuItem("Football", "Experimentar")); // 63
menu_camisetas.AddItem(API.createMenuItem("Polo 5", "Experimentar")); // 65
menu_camisetas.AddItem(API.createMenuItem("Polo 5 (Para dentro)", "Experimentar")); // 66
menu_camisetas.AddItem(API.createMenuItem("Social dobrada", "Experimentar")); // 67
menu_camisetas.AddItem(API.createMenuItem("Estampada", "Experimentar")); // 69
menu_camisetas.AddItem(API.createMenuItem("Lisa 9", "Experimentar")); // 71
menu_camisetas.AddItem(API.createMenuItem("Corredor", "Experimentar")); // 73
menu_camisetas.AddItem(API.createMenuItem("Corredor 2", "Experimentar")); // 75
menu_camisetas.AddItem(API.createMenuItem("Com neon", "Experimentar")); // 79
menu_camisetas.AddItem(API.createMenuItem("Larga com estampa", "Experimentar")); // 85
menu_camisetas.AddItem(API.createMenuItem("Natalina 2", "Experimentar")); // 86
menu_camisetas.AddItem(API.createMenuItem("Natalina 3", "Experimentar")); // 87
menu_camisetas.AddItem(API.createMenuItem("Com neon 2", "Experimentar")); // 89
menu_camisetas.AddItem(API.createMenuItem("Colete com gorro", "Experimentar")); // 90
menu_camisetas.AddItem(API.createMenuItem("Colete com gorro", "Experimentar")); // 94
menu_camisetas.AddItem(API.createMenuItem("Natalina 4", "Experimentar")); // 87

//-- [CASACOS/MOLETONS] --
var menu_casmole = API.createMenu("Binco", "Selecione um casaco!", 0, 0, 6);

menu_casmole.AddItem(API.createMenuItem("De Piloto", "Experimentar")); // 24
menu_casmole.AddItem(API.createMenuItem("Moleton de la", "Experimentar")); // 25
menu_casmole.AddItem(API.createMenuItem("Paraquedista", "Experimentar")); // 27
menu_casmole.AddItem(API.createMenuItem("Simples", "Experimentar")); // 29
menu_casmole.AddItem(API.createMenuItem("Basico", "Experimentar")); // 30
menu_casmole.AddItem(API.createMenuItem("Ostentador (Casaco)", "Experimentar")); // 36
menu_casmole.AddItem(API.createMenuItem("Ostentador (Moletom)", "Experimentar")); // 37
menu_casmole.AddItem(API.createMenuItem("College", "Experimentar")); // 38
menu_casmole.AddItem(API.createMenuItem("Baseball", "Experimentar")); // 43
menu_casmole.AddItem(API.createMenuItem("Liso (Casaco)", "Experimentar")); // 45
menu_casmole.AddItem(API.createMenuItem("College 2", "Experimentar")); // 46
menu_casmole.AddItem(API.createMenuItem("Liso (Moletom)", "Experimentar")); // 47
menu_casmole.AddItem(API.createMenuItem("College Liso", "Experimentar")); // 48
menu_casmole.AddItem(API.createMenuItem("Moletom do time", "Experimentar")); // 52
menu_casmole.AddItem(API.createMenuItem("Sobretudo de Couro", "Experimentar")); // 57
menu_casmole.AddItem(API.createMenuItem("Manga Longa Lisa", "Experimentar")); // 58
menu_casmole.AddItem(API.createMenuItem("Listrado", "Experimentar")); // 59
menu_casmole.AddItem(API.createMenuItem("Casaco H", "Experimentar")); // 61
menu_casmole.AddItem(API.createMenuItem("SecureServ", "Experimentar")); // 64
menu_casmole.AddItem(API.createMenuItem("Moletom do time 2", "Experimentar")); // 68
menu_casmole.AddItem(API.createMenuItem("College Liso 2", "Experimentar")); // 70
menu_casmole.AddItem(API.createMenuItem("Com colete", "Experimentar")); // 72
menu_casmole.AddItem(API.createMenuItem("Desenhada", "Experimentar")); // 74
menu_casmole.AddItem(API.createMenuItem("Basico 1", "Experimentar")); // 76
menu_casmole.AddItem(API.createMenuItem("Basico 2", "Experimentar")); // 77
menu_casmole.AddItem(API.createMenuItem("Simples 2", "Experimentar")); // 78
menu_casmole.AddItem(API.createMenuItem("Simples 3", "Experimentar")); // 80
menu_casmole.AddItem(API.createMenuItem("Contra Chuva", "Experimentar")); // 81
menu_casmole.AddItem(API.createMenuItem("Casaco longo", "Experimentar")); // 82
menu_casmole.AddItem(API.createMenuItem("Contra Chuva 2", "Experimentar")); // 83
menu_casmole.AddItem(API.createMenuItem("Ostentador 2 (Moletom)", "Experimentar")); // 84
menu_casmole.AddItem(API.createMenuItem("Moletom estampado", "Experimentar")); // 88
menu_casmole.AddItem(API.createMenuItem("Moletom estampado com gorro", "Experimentar")); // 91
menu_casmole.AddItem(API.createMenuItem("Casaco com gorro", "Experimentar")); // 93
menu_casmole.AddItem(API.createMenuItem("Moletom Natalino", "Experimentar")); // 94

//-- [Bermudas] --
var menu_bermudas = API.createMenu("Binco", "Selecione uma bermuda!", 0, 0, 6);

menu_bermudas.AddItem(API.createMenuItem("Básica", "Experimentar")); // 0
menu_bermudas.AddItem(API.createMenuItem("Básica 2", "Experimentar")); // 1
menu_bermudas.AddItem(API.createMenuItem("de Corrida", "Experimentar")); // 2
menu_bermudas.AddItem(API.createMenuItem("Largada", "Experimentar")); // 3
menu_bermudas.AddItem(API.createMenuItem("Tectel", "Experimentar")); // 4
menu_bermudas.AddItem(API.createMenuItem("Lisa", "Experimentar")); // 5
menu_bermudas.AddItem(API.createMenuItem("de Corrida 2", "Experimentar")); // 6
menu_bermudas.AddItem(API.createMenuItem("Cueca Box", "Experimentar")); // 7
menu_bermudas.AddItem(API.createMenuItem("Bermudão", "Experimentar")); // 8
menu_bermudas.AddItem(API.createMenuItem("Tectel 2", "Experimentar")); // 9
menu_bermudas.AddItem(API.createMenuItem("Toalha", "Experimentar")); // 10
menu_bermudas.AddItem(API.createMenuItem("Cueca Box 2", "Experimentar")); // 11
menu_bermudas.AddItem(API.createMenuItem("Bermudão 2", "Experimentar")); // 12

//-- [Calças] --
var menu_calcas = API.createMenu("Binco", "Selecione uma Calça!", 0, 0, 6);

menu_calcas.AddItem(API.createMenuItem("Jeans Básica", "Experimentar")); // 0
menu_calcas.AddItem(API.createMenuItem("Jeans Larga", "Experimentar")); // 1
menu_calcas.AddItem(API.createMenuItem("Moletom", "Experimentar")); // 3
menu_calcas.AddItem(API.createMenuItem("Jeans Skinny", "Experimentar")); // 4
menu_calcas.AddItem(API.createMenuItem("Moletom Larga", "Experimentar")); // 5
menu_calcas.AddItem(API.createMenuItem("Moletom Larga 2", "Experimentar")); // 7
menu_calcas.AddItem(API.createMenuItem("Justa", "Experimentar")); // 8
menu_calcas.AddItem(API.createMenuItem("Justa + Cinto", "Experimentar")); // 9
menu_calcas.AddItem(API.createMenuItem("Pré-Social", "Experimentar")); // 10
menu_calcas.AddItem(API.createMenuItem("Pré-Social 2", "Experimentar")); // 13
menu_calcas.AddItem(API.createMenuItem("A elegante", "Experimentar")); // 19
menu_calcas.AddItem(API.createMenuItem("Bem Vestido", "Experimentar")); // 20
menu_calcas.AddItem(API.createMenuItem("De boas + Cinto", "Experimentar")); // 22
menu_calcas.AddItem(API.createMenuItem("De boas + Cinto 2", "Experimentar")); // 23
menu_calcas.AddItem(API.createMenuItem("Justa + Cinto", "Experimentar")); // 24
menu_calcas.AddItem(API.createMenuItem("Justa + Cinto 2", "Experimentar")); // 25
menu_calcas.AddItem(API.createMenuItem("Camuflada Justa", "Experimentar")); // 26
menu_calcas.AddItem(API.createMenuItem("Larga Cool", "Experimentar")); // 27
menu_calcas.AddItem(API.createMenuItem("Arrumadinho", "Experimentar")); // 28
menu_calcas.AddItem(API.createMenuItem("Listrada", "Experimentar")); // 29
menu_calcas.AddItem(API.createMenuItem("De saltador", "Experimentar")); // 30
menu_calcas.AddItem(API.createMenuItem("De Corrida", "Experimentar")); // 32
menu_calcas.AddItem(API.createMenuItem("De piloto", "Experimentar")); // 34
menu_calcas.AddItem(API.createMenuItem("Bem vestido 2", "Experimentar")); // 35
menu_calcas.AddItem(API.createMenuItem("Funcionário Público", "Experimentar")); // 36
menu_calcas.AddItem(API.createMenuItem("Larga + Cinto", "Experimentar")); // 37
menu_calcas.AddItem(API.createMenuItem("Tranquila", "Experimentar")); // 38
menu_calcas.AddItem(API.createMenuItem("Tranquila Alta", "Experimentar")); // 39
menu_calcas.AddItem(API.createMenuItem("Saltador", "Experimentar")); // 41
menu_calcas.AddItem(API.createMenuItem("Jeans Largona", "Experimentar")); // 43
menu_calcas.AddItem(API.createMenuItem("De couro Justa", "Experimentar")); // 45
menu_calcas.AddItem(API.createMenuItem("Cargo Larga", "Experimentar")); // 46
menu_calcas.AddItem(API.createMenuItem("Elegante", "Experimentar")); // 47
menu_calcas.AddItem(API.createMenuItem("Social Normal", "Experimentar")); // 48
menu_calcas.AddItem(API.createMenuItem("Social Justa", "Experimentar")); // 49
menu_calcas.AddItem(API.createMenuItem("Couro Normal", "Experimentar")); // 50
menu_calcas.AddItem(API.createMenuItem("Ostentador", "Experimentar")); // 51
menu_calcas.AddItem(API.createMenuItem("Couro Justo", "Experimentar")); // 52
menu_calcas.AddItem(API.createMenuItem("Ostentador + Cinto", "Experimentar")); // 53
menu_calcas.AddItem(API.createMenuItem("Moletom com listra", "Experimentar")); // 55
menu_calcas.AddItem(API.createMenuItem("Quadriculada", "Experimentar")); // 58
menu_calcas.AddItem(API.createMenuItem("Listrada 2", "Experimentar")); // 60
menu_calcas.AddItem(API.createMenuItem("Jeans Old", "Experimentar")); // 63
menu_calcas.AddItem(API.createMenuItem("Moletom das ruas", "Experimentar")); // 64
menu_calcas.AddItem(API.createMenuItem("Tipo Pijama", "Experimentar")); // 65
menu_calcas.AddItem(API.createMenuItem("Corredor 1", "Experimentar")); // 66
menu_calcas.AddItem(API.createMenuItem("Corredor 2", "Experimentar")); // 67
menu_calcas.AddItem(API.createMenuItem("Rockstar", "Experimentar")); // 68
menu_calcas.AddItem(API.createMenuItem("Tipo Pijama 2", "Experimentar")); // 69
menu_calcas.AddItem(API.createMenuItem("Rockstar 2", "Experimentar")); // 70
menu_calcas.AddItem(API.createMenuItem("Couro Cool", "Experimentar")); // 71
menu_calcas.AddItem(API.createMenuItem("Couro Larga", "Experimentar")); // 73
menu_calcas.AddItem(API.createMenuItem("Jeans + Cinto", "Experimentar")); // 75
menu_calcas.AddItem(API.createMenuItem("Jeans Old", "Experimentar")); // 76
menu_calcas.AddItem(API.createMenuItem("de Neon", "Experimentar")); // 77
menu_calcas.AddItem(API.createMenuItem("SWAG 1", "Experimentar")); // 78
menu_calcas.AddItem(API.createMenuItem("SWAG 2", "Experimentar")); // 79
menu_calcas.AddItem(API.createMenuItem("SWAG 3", "Experimentar")); // 80
menu_calcas.AddItem(API.createMenuItem("SWAG 4", "Experimentar")); // 81
menu_calcas.AddItem(API.createMenuItem("SWAG 5", "Experimentar")); // 82
menu_calcas.AddItem(API.createMenuItem("SWAG 6", "Experimentar")); // 83
menu_calcas.AddItem(API.createMenuItem("Corredor 3", "Experimentar")); // 85

//-- [CALÇADOS] --

var menu_calcado = API.createMenu("Binco", "Selecione um calçado!", 0, 0, 6);

menu_calcado.AddItem(API.createMenuItem("Bota", "Experimentar")); // 0
menu_calcado.AddItem(API.createMenuItem("Skatista 1", "Experimentar")); // 1
menu_calcado.AddItem(API.createMenuItem("Corredor", "Experimentar")); // 2
menu_calcado.AddItem(API.createMenuItem("Elegante", "Experimentar")); // 3
menu_calcado.AddItem(API.createMenuItem("All Stars Cano Alto", "Experimentar")); // 4
menu_calcado.AddItem(API.createMenuItem("Chinelo", "Experimentar")); // 5
menu_calcado.AddItem(API.createMenuItem("Chinelo com meia", "Experimentar")); // 6
menu_calcado.AddItem(API.createMenuItem("Skatista Simples", "Experimentar")); // 7
menu_calcado.AddItem(API.createMenuItem("Skatista Básico", "Experimentar")); // 8
menu_calcado.AddItem(API.createMenuItem("Skatista com logo", "Experimentar")); // 9
menu_calcado.AddItem(API.createMenuItem("Social com meias", "Experimentar")); // 10
menu_calcado.AddItem(API.createMenuItem("Sofisticado", "Experimentar")); // 11
menu_calcado.AddItem(API.createMenuItem("Bota 2", "Experimentar")); // 12
menu_calcado.AddItem(API.createMenuItem("Bota 3", "Experimentar")); // 14
menu_calcado.AddItem(API.createMenuItem("Bota 4", "Experimentar")); // 15
menu_calcado.AddItem(API.createMenuItem("Chinelo 2", "Experimentar")); // 16
menu_calcado.AddItem(API.createMenuItem("Natalino", "Experimentar")); // 17
menu_calcado.AddItem(API.createMenuItem("Social Festivo", "Experimentar")); // 18
menu_calcado.AddItem(API.createMenuItem("Casual-Social", "Experimentar")); // 19
menu_calcado.AddItem(API.createMenuItem("Social com meias 2", "Experimentar")); // 20
menu_calcado.AddItem(API.createMenuItem("Social", "Experimentar")); // 21
menu_calcado.AddItem(API.createMenuItem("All Stars cano Alto 2", "Experimentar")); // 22
menu_calcado.AddItem(API.createMenuItem("Simples", "Experimentar")); // 23
menu_calcado.AddItem(API.createMenuItem("Botina", "Experimentar")); // 24
menu_calcado.AddItem(API.createMenuItem("Botina 2", "Experimentar")); // 25
menu_calcado.AddItem(API.createMenuItem("All Stars", "Experimentar")); // 26
menu_calcado.AddItem(API.createMenuItem("Botina 3", "Experimentar")); // 27
menu_calcado.AddItem(API.createMenuItem("Espinhoso", "Experimentar")); // 28
menu_calcado.AddItem(API.createMenuItem("Swag Dourado", "Experimentar")); // 29
menu_calcado.AddItem(API.createMenuItem("Sapatilha", "Experimentar")); // 30
menu_calcado.AddItem(API.createMenuItem("Corredor 2", "Experimentar")); // 31
menu_calcado.AddItem(API.createMenuItem("Corredor cano Alto", "Experimentar")); // 32
menu_calcado.AddItem(API.createMenuItem("Botina 4", "Experimentar")); // 35
menu_calcado.AddItem(API.createMenuItem("Sapatilha 2", "Experimentar")); // 36
menu_calcado.AddItem(API.createMenuItem("Bota Country", "Experimentar")); // 37
menu_calcado.AddItem(API.createMenuItem("Country", "Experimentar")); // 38
menu_calcado.AddItem(API.createMenuItem("Semi-Social com meia", "Experimentar")); // 39
menu_calcado.AddItem(API.createMenuItem("Sapatilha 3", "Experimentar")); // 40
menu_calcado.AddItem(API.createMenuItem("Pans", "Experimentar")); // 41
menu_calcado.AddItem(API.createMenuItem("Botinha", "Experimentar")); // 42
menu_calcado.AddItem(API.createMenuItem("Bota Country 2", "Experimentar")); // 43
menu_calcado.AddItem(API.createMenuItem("Country 2", "Experimentar")); // 44
menu_calcado.AddItem(API.createMenuItem("BasketBall", "Experimentar")); // 46
menu_calcado.AddItem(API.createMenuItem("Botona", "Experimentar")); // 47
menu_calcado.AddItem(API.createMenuItem("All Stars 2", "Experimentar")); // 48
menu_calcado.AddItem(API.createMenuItem("All Stars 3", "Experimentar")); // 49
menu_calcado.AddItem(API.createMenuItem("Bota da Lama", "Experimentar")); // 50
menu_calcado.AddItem(API.createMenuItem("Sapato", "Experimentar")); // 51
menu_calcado.AddItem(API.createMenuItem("Semi-Bota", "Experimentar")); // 52
menu_calcado.AddItem(API.createMenuItem("Botina 5", "Experimentar")); // 53
menu_calcado.AddItem(API.createMenuItem("Sapato 2", "Experimentar")); // 54
menu_calcado.AddItem(API.createMenuItem("Iluminado", "Experimentar")); // 55
menu_calcado.AddItem(API.createMenuItem("Sapato 3", "Experimentar")); // 56
menu_calcado.AddItem(API.createMenuItem("Cano Alto", "Experimentar")); // 57
menu_calcado.AddItem(API.createMenuItem("Sapatilha 4", "Experimentar")); // 58

var itemSelecionado = -1;
var itemMenu = 0;
var spamProtection = false;
var texturaEscolhida = 0;
var ANTI_SPAM_TIME = 200;

var list_textura = new List(String);
list_textura.Add("0");
list_textura.Add("1");
list_textura.Add("2");
list_textura.Add("3");
list_textura.Add("4");
list_textura.Add("5");
list_textura.Add("6");
list_textura.Add("7");
list_textura.Add("8");
list_textura.Add("9");
list_textura.Add("10");
list_textura.Add("11");
list_textura.Add("12");
list_textura.Add("13");
list_textura.Add("14");
list_textura.Add("15");
list_textura.Add("16");

var TextureMenu = API.createListItem("Textura", "Mude a textura/cor/estampa da roupa selecionada", list_textura, 0);
var CachorroLonge = 0;
var mydogBlip;

var g_menu_confirma = API.createMenu("Binco", "Você quer comprar essa roupa?", 0, 0, 6);
g_menu_confirma.ResetKey(menuControl.Back);

g_menu_confirma.AddItem(TextureMenu);
g_menu_confirma.AddItem(API.createMenuItem("Comprar roupa", "Clique para comprar."));
g_menu_confirma.AddItem(API.createMenuItem("Voltar", "Clique para voltar ao menu."));
/* ========================================================================= */
API.onUpdate.connect(function() 
{
    var player = API.getLocalPlayer();

    if (!API.hasEntitySyncedData(player, "logado"))  return;
    if (API.getEntitySyncedData(player, "toghud"))  return;

    //=================================================
    //Personagem Custom - Freeze
    //API.drawMenu(menu_camisetas);
    //API.drawMenu(menu_casmole);
    //API.drawMenu(g_menu_confirma);
    //API.drawMenu(menu_bermudas);
    //API.drawMenu(menu_calcas);
    //API.drawMenu(menu_calcado);
    //Criando Personagem
    //API.drawMenu(GenderMenu);
    //API.drawMenu(criar_pers);
    //=================================================
    if (API.isPlayerInAnyVehicle(player))
    {
        var car_rot = API.getEntityRotation(API.getPlayerVehicle(player));
        

        if(car_rot.Y >= 50 || car_rot.Y <= -50)
        {
            API.disableControlThisFrame(107);
        }
    }
    //=================================================
    //Desabilitar a troca de arma pelo Capslock ou mouse whell. (Armas Scroll)
    API.disableControlThisFrame(12);
    API.disableControlThisFrame(13);
    API.disableControlThisFrame(14);
    API.disableControlThisFrame(15);
    API.disableControlThisFrame(16);
    API.disableControlThisFrame(17);

    //VehicleFly
    API.disableControlThisFrame(87);
    API.disableControlThisFrame(88);
    API.disableControlThisFrame(89);
    API.disableControlThisFrame(90);

    API.disableControlThisFrame(94);//VehicleStuntUpDown

    //VehicleCinematic
    API.disableControlThisFrame(95);
    API.disableControlThisFrame(96);
    API.disableControlThisFrame(97);
    API.disableControlThisFrame(98);

    //
    API.disableControlThisFrame(107);
    API.disableControlThisFrame(108);
    API.disableControlThisFrame(109);
    API.disableControlThisFrame(110);
    API.disableControlThisFrame(111);
    API.disableControlThisFrame(112);
    API.disableControlThisFrame(113);

    if (API.isEntityUpsidedown(API.getPlayerVehicle(player)))
    {
        API.disableControlThisFrame(59);
    }
    //=================================================
    API.drawText("$" + API.getEntitySyncedData(player, "dinheiro"), resR.Width - 15, 45, 0.5, 115, 186, 131, 255, 7, 2, false, true, 0);
    API.drawText(API.getWorldSyncedData("horario"), resR.Width - 15, 70, 0.4, 255, 255, 255, 255, 0, 2, false, true, 0);

    if (API.isPlayerInAnyVehicle(player))
    {
        var veh = API.getPlayerVehicle(player);
        var velocity = API.getEntityVelocity(veh);
        var speed = Math.sqrt(
				velocity.X * velocity.X +
				velocity.Y * velocity.Y +
				velocity.Z * velocity.Z
				);

        var kmh = Math.round(speed * 3.6);
        
        var Max_Player_Speed = 999;
        //if (API.getEntitySyncedData(player, "carLic") == 0) Max_Player_Speed = 50;
        if(API.getEntitySyncedData(player, "VelMax") != 0) Max_Player_Speed = API.getEntitySyncedData(player, "VelMax");

        API.callNative("SET_ENTITY_MAX_SPEED", veh, (Max_Player_Speed / 3.6));

        var kmhShow = `_`;
        if (kmh > 70 && kmh < 95) {
            kmhShow = `~y~${kmh}`;
        }
        else if (kmh >= 95)
            kmhShow = `~r~${kmh}`;
        else
            kmhShow = `${kmh}`;

        API.drawText(`${kmhShow} km/h`, 420, resR.Height - 90, 0.6, 85, 85, 85, 255, 4, 2, false, true, 0);
        API.dxDrawTexture("Client\\img\\fuel.png", new Point(Math.round(330), Math.round(resR.Height - 50)), new Size(32, 32));
        //API.drawText("~g~|||||||~w~|||", 365, resR.Height - 50, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);   
        API.drawText(`${VehGas} L`, 365, resR.Height - 50, 0.5, 255, 255, 255, 255, 4, 0, false, true, 0);
        
    }
    //==================================================
    //==================================================
    //Empregos
    if (InBusStop) {
        API.drawText(garbagePickedUp, res_X - 15, res_Y - 230, 0.9, 115, 186, 131, 255, 4, 2, false, true, 0);
        API.drawText(`${pontoAtual} / ${PontosRestantes}`, res_X - 15, res_Y - 180, 1, 255, 255, 255, 255, 4, 2, false, true, 0);
    }
    if (Congelado > 0) {
        if (Congelado > 1)
            Congelado--;
        else {
            Congelado = 0;
            API.setEntityPositionFrozen(VehCongelado, false);
        }
    }
    //Cachorro seguindo o dono
    var TemMyDog = 0;
    var DogIndoAtacar = 0;
    if (API.hasEntitySyncedData(API.getLocalPlayer(), "mydog"))
        myDog = API.getEntitySyncedData(API.getLocalPlayer(), "mydog");
    if (API.hasEntitySyncedData(API.getLocalPlayer(), "TemDog"))
        TemMyDog = API.getEntitySyncedData(API.getLocalPlayer(), "TemDog");
    if (API.hasEntitySyncedData(API.getLocalPlayer(), "DogIndoAtacar"))
        DogIndoAtacar = API.getEntitySyncedData(API.getLocalPlayer(), "DogIndoAtacar");

    if (myDog != null && TemMyDog != 0 && DogIndoAtacar == "nc")
    {
        var posDog = API.getEntityPosition(myDog); // ponto A
        var PlayerPosi = API.getEntityPosition(player); // Ponto B

        
        API.triggerServerEvent("set_dogpos", 1, posDog);

        if (API.hasEntitySyncedData(API.getLocalPlayer(), "TemDogBlip")) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "TemDogBlip") != 1) {
                mydogBlip = API.createBlip(posDog);
                API.setEntitySyncedData(API.getLocalPlayer(), "TemDogBlip", 1);
                API.setBlipSprite(mydogBlip, 273);
                API.setBlipName(mydogBlip, API.getEntitySyncedData(API.getLocalPlayer(), "NomeDoDog"));
            }
            else
            {
                API.setBlipPosition(mydogBlip, posDog);
            }
        }
        else
        {
            mydogBlip = API.createBlip(posDog);
            API.setEntitySyncedData(API.getLocalPlayer(), "TemDogBlip", 1);
            API.setBlipSprite(mydogBlip, 273);
            API.setBlipName(mydogBlip, API.getEntitySyncedData(API.getLocalPlayer(), "NomeDoDog"));
        }
    }
    else {
        if (API.hasEntitySyncedData(API.getLocalPlayer(), "TemDogBlip")) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "TemDogBlip") == 1) {
                API.deleteEntity(mydogBlip);
                API.setEntitySyncedData(API.getLocalPlayer(), "TemDogBlip", 0);
            }
        }
    }
    //Cachorro seguindo o alguém para atacar
    var Dog_MeTacando = null;
    if (API.hasEntitySyncedData(API.getLocalPlayer(), "DogVindoMeAtacar"))
        Dog_MeTacando = API.getEntitySyncedData(API.getLocalPlayer(), "DogVindoMeAtacar");
    
    if (Dog_MeTacando != null)
    {
        var posDog = API.getEntityPosition(Dog_MeTacando);

        API.triggerServerEvent("set_dogpos", 2, posDog);
    }
});

/*API.onPlayerEnterVehicle.connect(function (veh) {
    //Cachorro

    var TemMyDog = 0;
    if (API.hasEntitySyncedData(API.getLocalPlayer(), "mydog"))
        myDog = API.getEntitySyncedData(API.getLocalPlayer(), "mydog");
    if (API.hasEntitySyncedData(API.getLocalPlayer(), "TemDog"))
        TemMyDog = API.getEntitySyncedData(API.getLocalPlayer(), "TemDog");


    if (myDog != null && TemMyDog != 0) {

        API.sendNotification("Cachorro entrando no veículo.");
        API.callNative("TASK_ENTER_VEHICLE", myDog, veh, 4000, 0, 1.0, 3, 0);
    }
});*/

var VehGas = 0;

API.onServerEventTrigger.connect(function(eventName, args) 
{
    param1 = null;
    param2 = null;
    if (eventName === "showShard") 
    {
        API.showShard(args[0], parseInt(args[1]));
    }
    else if(eventName ==="gasolinaVeh")
    {
        VehGas = parseFloat(args[0]).toFixed(2);
    }
    else if (eventName === "showSubtitle") {
        API.displaySubtitle(args[0], parseInt(args[1]));
    }
    else if (eventName === "ligarvisao") {
        API.callNative(args[0], true);
    }
    else if (eventName === "desligarvisao") {
        API.callNative(args[0], false);
    }
    else if (eventName === "cancelarconce") {
        AcaoVeiculoConce("cancelar");
    }
    else if (eventName === "player_login") {
        loginBrowser = API.createCefBrowser(550, 425);
        const leftPos = (res.Width / 2) - (550 / 2);
        const topPos = (res.Height / 2) - (425 / 2);
        API.setCefBrowserPosition(loginBrowser, leftPos, topPos);
        API.loadPageCefBrowser(loginBrowser, "Client/login.html");
        API.waitUntilCefBrowserInit(loginBrowser);
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para autenticar através do comando caso o dialog não apareça em 10 segundos.");
    }
    else if (eventName === "tab_players") {
        if (playersBrowser != null)
            return;

        param1 = args[0];
        param2 = args[1];

        playersBrowser = API.createCefBrowser(1000, 800);
        const leftPos = (res.Width / 2) - (1000 / 2);
        const topPos = (res.Height / 2) - (800 / 2);
        API.waitUntilCefBrowserInit(playersBrowser);
        API.setCefBrowserPosition(playersBrowser, leftPos, topPos);
        API.loadPageCefBrowser(playersBrowser, "Client/players.html");
        API.showCursor(true);
        API.setCanOpenChat(false);

        API.sendNotification("Pressione F1 para fechar o dialog.");
    }
    else if (eventName === "criarcamera") {
        var pos = args[0];
        var target = args[1];

        var newCam = API.createCamera(pos, new Vector3());
        API.pointCameraAtPosition(newCam, target);
        API.setActiveCamera(newCam);
    }
    else if (eventName === "limparcamera") {
        API.setActiveCamera(null);
    }
    else if (eventName === "player_stats") {
        if (statsBrowser != null)
            return;

        param1 = args[0];

        statsBrowser = API.createCefBrowser(800, 600);
        const leftPos = (res.Width / 2) - (800 / 2);
        const topPos = (res.Height / 2) - (600 / 2);
        API.setCefBrowserPosition(statsBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(statsBrowser);
        API.loadPageCefBrowser(statsBrowser, "Client/stats.html");
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para fechar o dialog.");
    }
    else if (eventName === "player_ban") {
        param1 = args[0];

        banBrowser = API.createCefBrowser(550, 500);
        const leftPos = (res.Width / 2) - (550 / 2);
        const topPos = (res.Height / 2) - (500 / 2);
        API.setCefBrowserPosition(banBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(banBrowser);
        API.loadPageCefBrowser(banBrowser, "Client/ban.html");
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para sair do servidor.");
    }
    else if (eventName === "listarsos") {
        if (sosBrowser != null)
            return;

        param1 = args[0];
        param2 = args[1];

        sosBrowser = API.createCefBrowser(1280, 720);
        const leftPos = (res.Width / 2) - (1280 / 2);
        const topPos = (res.Height / 2) - (720 / 2);
        API.setCefBrowserPosition(sosBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(sosBrowser);
        API.loadPageCefBrowser(sosBrowser, "Client/sos.html");
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para fechar o dialog.");
    }
    else if (eventName === "conce") {
        if (conceBrowser != null)
            return;

        param1 = args[0];

        conceBrowser = API.createCefBrowser(1280, 750);
        const leftPos = (res.Width / 2) - (1280 / 2);
        const topPos = (res.Height / 2) - (750 / 2);
        API.setCefBrowserPosition(conceBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(conceBrowser);
        API.loadPageCefBrowser(conceBrowser, "Client/conce.html");
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para fechar o dialog.");

    }
    else if (eventName === "menusemresposta") {
        //menuPool = API.getMenuPool();
        var menu = API.createMenu(args[0], args[1], 0, 0, args[2]);

        var list = args[3];
        var list2 = args[4];
        for (var i = 0; i < list.Count; i++) {
            menu.AddItem(API.createMenuItem(list[i], list2[i]));
        }

       // menuPool.Add(menu);

        menu.OnItemSelect.connect(function (sender, item, index) {
            API.showCursor(false);
            menu.Visible = false;
        });

        menu.Visible = true;
        //menuPool.ProcessMenus();
    }
    else if (eventName === "menucomresposta") {
        //menuPool = API.getMenuPool();
        var menu = API.createMenu(args[0], args[1], 0, 0, args[2]);

        var list = args[3];
        var list2 = args[4];
        for (var i = 0; i < list.Count; i++)
            menu.AddItem(API.createMenuItem(list[i], list2[i]));

        //menuPool.Add(menu);

        menu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("respostamenu", args[0], item.Text);

            API.showCursor(false);
            menu.Visible = false;
        });

        menu.Visible = true;
        //menuPool.ProcessMenus();
    }
    else if (eventName === "comprarvariacao") {
        //menuPool = API.getMenuPool();
        var menu = API.createMenu("Variações", "", 0, 0, 6);

        var list = new List(String);
        for (var i = 0; i <= 11; i++)
            list.Add(i.toString());

        var slot = 0;
        var drawable = 0;
        var texture = 0;

        var iComponente = API.createListItem("Componente", "Selecione o componente", list, 0);
        var iDesenho = API.createListItem("Desenho", "Selecione o desenho", list, 0);
        var iTextura = API.createListItem("Textura", "Selecione a textura", list, 0);

        menu.AddItem(iComponente);
        menu.AddItem(iDesenho);
        menu.AddItem(iTextura);

        menu.AddItem(API.createMenuItem("Visualizar", ""));
        menu.AddItem(API.createMenuItem("Confirmar", ""));
        menu.AddItem(API.createMenuItem("Cancelar", ""));

        iComponente.OnListChanged.connect(function (sender, new_index) {
            slot = new_index;
        });

        iDesenho.OnListChanged.connect(function (sender, new_index) {
            drawable = new_index;
        });

        iTextura.OnListChanged.connect(function (sender, new_index) {
            texture = new_index;
        });

        //menuPool.Add(menu);

        menu.OnItemSelect.connect(function (sender, item, index) {
            if (item.Text == "Visualizar") {
                API.triggerServerEvent("alterarvariacao", slot, drawable, texture);
            }
            else if (item.Text == "Confirmar") {
                API.showCursor(false);
                menu.Visible = false;

                API.triggerServerEvent("confirmarvariacoes");
            }
            else if (item.Text == "Cancelar") {
                API.showCursor(false);
                menu.Visible = false;

                API.triggerServerEvent("resetarvariacoes");
            }
        });

        menu.Visible = true;
        //menuPool.ProcessMenus();
    }
    else if (eventName === "listarpersonagens") {
        //menuPool = API.getMenuPool();
        var menu = API.createMenu("Personagens", "Lista de Personagens", 0, 0, 6);

        var list = args[0];
        var pers = new List(String);
        for (var i = 0; i < list.Count; i++)
            pers.Add(list[i]);

        var personagem = list[0].toString();

        var personagens = API.createListItem("Personagens", "Selecione o personagem", pers, 0);
        menu.AddItem(personagens);
        menu.AddItem(API.createMenuItem("Confirmar", ""));

        personagens.OnListChanged.connect(function (sender, new_index) {
            personagem = list[new_index].toString();
            API.triggerServerEvent("alterarpersonagem", new_index);
        });

        //menuPool.Add(menu);

        menu.OnItemSelect.connect(function (sender, item, index) {
            if (item.Text == "Confirmar") {
                API.showCursor(false);
                menu.Visible = false;

                API.triggerServerEvent("confirmarpersonagem", personagem);
            }
        });

        menu.Visible = true;
        //menuPool.ProcessMenus();
    }
    else if (eventName === "player_ragdoll") {
        param1 = args[0];
        API.setPedToRagdoll(-1, param1);
    }
    else if (eventName === "player_ragdoll_c") {
        API.cancelPedRagdoll();
    }
    else if (eventName === "player_help") {
        if (helpBrowser != null)
            return;

        param1 = args[0];

        helpBrowser = API.createCefBrowser(800, 600);
        const leftPos = (res.Width / 2) - (800 / 2);
        const topPos = (res.Height / 2) - (600 / 2);
        API.setCefBrowserPosition(helpBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(helpBrowser);
        API.loadPageCefBrowser(helpBrowser, "Client/help.html");
        API.showCursor(true);
        API.setCanOpenChat(false);

        API.sendNotification("Pressione F1 para fechar o dialog.");
    }
    else if (eventName === "atm") {
        if (atmBrowser != null)
            return;

        param1 = args[0];

        atmBrowser = API.createCefBrowser(800, 600);
        const leftPos = (res.Width / 2) - (800 / 2);
        const topPos = (res.Height / 2) - (600 / 2);
        API.setCefBrowserPosition(atmBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(atmBrowser);
        API.loadPageCefBrowser(atmBrowser, "Client/atm.html");
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para fechar o dialog.");
    }
    else if (eventName === "uniformes") {
        //menuPool = API.getMenuPool();
        var menu = API.createMenu("CUSTOMIZAÇÃO", "", 0, 0, 6);

        var face = "0";
        var listFace = new List(String);
        var listCabelo = new List(String);
        var listCamisa = new List(String);
        var listColete = new List(String);
        var listCobertura = new List(String);
        var listOculos = new List(String);

        if (args[0].toLowerCase() == "cop01smy") {
            listFace.Add("0.0.0");
            listFace.Add("0.0.1");
            listFace.Add("0.1.0");
            listFace.Add("0.1.1");
            listFace.Add("0.2.0");
            listFace.Add("0.2.1");

            listCamisa.Add("Manga Curta");
            listCamisa.Add("Manga Longa 3.1.0");

            listColete.Add("Sem Colete");
            listColete.Add("Colete 9.2.0");

            listCobertura.Add("Sem Cobertura");
            listCobertura.Add("Cobertura");

            listOculos.Add("Sem Óculos");
            listOculos.Add("1.0.0");
            listOculos.Add("1.2.0");
            listOculos.Add("1.3.0");
        }
        else if (args[0].toLowerCase() == "cop01sfy") {
            listFace.Add("0.0.0");
            listFace.Add("0.0.1");
            listFace.Add("0.1.0");
            listFace.Add("0.1.1");

            listCabelo.Add("2.0.0");
            listCabelo.Add("2.0.1");
            listCabelo.Add("2.1.0");
            listCabelo.Add("2.1.1");
            listCabelo.Add("2.2.0");
            listCabelo.Add("2.2.1");

            listCamisa.Add("Manga Curta");
            listCamisa.Add("Manga Longa 3.1.0");

            listCobertura.Add("Sem Cobertura");
            listCobertura.Add("Cobertura");

            listOculos.Add("Sem Óculos");
            listOculos.Add("1.0.0");
        }
        else if (args[0].toLowerCase() == "hwaycop01smy") {
            listFace.Add("0.0.0");
            listFace.Add("0.0.1");
            listFace.Add("0.0.2");
            listFace.Add("0.1.0");
            listFace.Add("0.1.1");
            listFace.Add("0.1.2");

            listCamisa.Add("Manga Curta");
            listCamisa.Add("Manga Longa 3.1.0");

            listCobertura.Add("Sem Cobertura");
            listCobertura.Add("Cobertura");

            listOculos.Add("Sem Óculos");
            listOculos.Add("1.0.0");
            listOculos.Add("1.1.0");
        }

        if (listFace.Count > 0) {
            var iFace = API.createListItem("Face", "", listFace, 0);
            menu.AddItem(iFace);
            iFace.OnListChanged.connect(function (sender, new_index) {
                var resf = listFace[new_index].split(".");
                face = resf[1];

                API.triggerServerEvent("alterarvariacao", resf[0].toString(), resf[1].toString(), resf[2].toString());
                API.triggerServerEvent("alterarvariacao", "3", "0", face.toString());
            });
        }

        if (listCabelo.Count > 0) {
            var iCabelo = API.createListItem("Cabelo", "", listCabelo, 0);
            menu.AddItem(iCabelo);
            iCabelo.OnListChanged.connect(function (sender, new_index) {
                var res = listCabelo[new_index].split(".");
                API.triggerServerEvent("alterarvariacao", res[0].toString(), res[1].toString(), res[2].toString());
            });
        }

        if (listCamisa.Count > 0) {
            var iCamisa = API.createListItem("Camisa", "", listCamisa, 0);
            menu.AddItem(iCamisa);
            iCamisa.OnListChanged.connect(function (sender, new_index) {
                if (new_index === 0)
                    API.triggerServerEvent("alterarvariacao", "3", "0", face.toString());
                else
                    API.triggerServerEvent("alterarvariacao", "3", "1", "0");
            });
        }

        if (listColete.Count > 0) {
            var iColete = API.createListItem("Colete", "", listColete, 0);
            menu.AddItem(iColete);
            iColete.OnListChanged.connect(function (sender, new_index) {
                if (new_index === 0)
                    API.triggerServerEvent("alterarvariacao", "9", "0", "0");
                else
                    API.triggerServerEvent("alterarvariacao", "9", "2", "0");
            });
        }

        if (listCobertura.Count > 0) {
            var iCobertura = API.createListItem("Cobertura", "", listCobertura, 0);
            menu.AddItem(iCobertura);
            iCobertura.OnListChanged.connect(function (sender, new_index) {
                if (new_index === 0)
                    API.triggerServerEvent("limparacessorio", "0");
                else
                    API.triggerServerEvent("alteraracessorio", "0", "0", "0");
            });
        }

        if (listOculos.Count > 0) {
            var iOculos = API.createListItem("Óculos", "", listOculos, 0);
            menu.AddItem(iOculos);
            iOculos.OnListChanged.connect(function (sender, new_index) {
                if (new_index === 0) {
                    API.triggerServerEvent("limparacessorio", "1");
                }
                else {
                    var res = listOculos[new_index].split(".");
                    API.triggerServerEvent("alteraracessorio", res[0].toString(), res[1].toString(), res[2].toString());
                }
            });
        }

        //menuPool.Add(menu);

        menu.Visible = true;
        //menuPool.ProcessMenus();
    }
    if (eventName == "UPDATE_CHARACTER_1") {
        LocalPlaya = args[0];
        setPedCharacter(args[0]);
    }
    if (eventName == "VoltarCamera") {
        API.setActiveCamera(null);
    }
    if (eventName == "CREATE_PERS") {

        if (args[0] == 1) {
            GenderMenu.Visible = true;
        }
        else {
            if (creatercharBrowser == null) {
                creatercharBrowser = API.createCefBrowser(res.Width, res.Height);
                API.waitUntilCefBrowserInit(creatercharBrowser);
                API.setCefBrowserPosition(creatercharBrowser, 0, 0);
                API.loadPageCefBrowser(creatercharBrowser, "Sistemas/cef/selecao_pers.html");

                API.showCursor(true);
            }
        }
    }
    if (eventName == "LoadSkinPers") {
        LocalPlaya = args[0];

        API.setEntitySyncedData(LocalPlaya, "GTAO_SHAPE_FIRST_ID", args[1]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_SHAPE_SECOND_ID", args[2]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_SKIN_FIRST_ID", args[3]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_SKIN_SECOND_ID", args[4]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_SHAPE_MIX", args[5]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_SKIN_MIX", args[6]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_HAIR_COLOR", args[7]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_HAIR_HIGHLIGHT_COLOR", args[8]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_EYE_COLOR", args[9]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_EYEBROWS", args[10]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_EYEBROWS_COLOR", args[11]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_MAKEUP_COLOR", args[12]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_LIPSTICK_COLOR", args[13]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_EYEBROWS_COLOR2", args[14]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_MAKEUP_COLOR2", args[15]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_LIPSTICK_COLOR2", args[16]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_HAIR_STYLE", args[17]);

        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_1", args[18]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_2", args[19]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_3", args[20]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_4", args[21]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_5", args[22]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_6", args[23]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_7", args[24]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_8", args[25]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_9", args[26]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_10", args[27]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_11", args[28]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_12", args[29]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_13", args[30]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_14", args[31]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_15", args[32]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_16", args[33]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_17", args[34]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_18", args[35]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_19", args[36]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_20", args[37]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_FACE_FEATURES_21", args[38]);
        API.setEntitySyncedData(LocalPlaya, "GTAO_HAS_CHARACTER_DATA", args[39]);

        setPedCharacter(args[0]);

    }
    if (eventName == "LoadSkinPers_login") {
        API.sendNotification("Aplicando skin Custom");
        var ent = args[0];
        //Pele/Mix
        API.callNative("SET_PED_HEAD_BLEND_DATA", ent, args[1], args[2], 0, args[3], args[4], 0, args[5], args[6], 0, false);
        //Cabelo
        API.setPlayerClothes(ent, 2, args[17], 0);
        API.callNative("_SET_PED_HAIR_COLOR", ent, args[7], args[8]);
        //Olhos
        API.callNative("_SET_PED_EYE_COLOR", ent, args[9]);
        //
        API.callNative("SET_PED_HEAD_OVERLAY", ent, 2, args[10], API.f(1));
        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 2, 1, args[11], args[14]);
        //Face Features
        API.callNative("_SET_PED_FACE_FEATURE", ent, 0, args[18]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 1, args[19]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 2, args[20]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 3, args[21]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 4, args[22]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 5, args[23]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 6, args[24]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 7, args[25]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 8, args[26]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 9, args[27]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 10, args[28]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 11, args[29]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 12, args[30]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 13, args[31]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 14, args[32]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 15, args[33]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 16, args[34]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 17, args[35]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 18, args[36]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 19, args[37]);
        API.callNative("_SET_PED_FACE_FEATURE", ent, 20, args[38]);

        API.sendNotification("Skin custom login aplicada");
    }
    if (eventName == "markonmap") {

        API.setWaypoint(args[0], args[1]);

    }
    if (eventName == "removemarkonmap") {

        API.removeWaypoint();

    }
    if (eventName == "bus_checkpoint") {
        //NextBusStop = new Vector3(args[0].X, args[0].Y, args[0].Z);
        API.setWaypoint(args[0].X, args[0].Y);

        var vector = args[0];
        JobHelper.createMarker("bus", vector, 3);

        if (args[1] != 0) {
            Congelado = 120;
            VehCongelado = API.getPlayerVehicle(API.getLocalPlayer());
            API.setEntityPositionFrozen(VehCongelado, true);
        }
        else {
            garbagePickedUp = "Va para o próximo ponto";
        }
    }
    if (eventName == "remove_job_marker") {
        var jobName = args[0];

        JobHelper.removeMarker(jobName);
    }
    if (eventName == "finalize_job_bus") {
        var jobName = args[0];

        JobHelper.removeMarker(jobName);
        API.removeWaypoint();
    }
    if (eventName == "update_bus_job") {
        InBusStop = args[0];
        pontoAtual = args[1];
        PontosRestantes = args[2];

    }
    if (eventName == "status_marker_job") {
        OnPickupJob = args[0];
    }
    if (eventName == "dentro_colshape") {
        PlayerInMarker = args[0];
    }
});

API.onResourceStop.connect(function () 
{
    if (loginBrowser != null) 
        API.destroyCefBrowser(loginBrowser);

    if (banBrowser != null) 
        API.destroyCefBrowser(banBrowser);

    if (helpBrowser != null) 
        API.destroyCefBrowser(helpBrowser);

    if (playersBrowser != null) 
        API.destroyCefBrowser(playersBrowser);

    if (statsBrowser != null) 
        API.destroyCefBrowser(statsBrowser);

    if (atmBrowser != null) 
        API.destroyCefBrowser(atmBrowser);

    if (conceBrowser != null)
        API.destroyCefBrowser(conceBrowser);

    if (autoescolaBrowser != null)
        API.destroyCefBrowser(autoescolaBrowser);
});

API.onKeyDown.connect(function(sender, keyEventArgs) 
{
    //Bloquear descapotar carro
    /*if (API.isEntityUpsidedown(API.getLocalPlayer()))
    {
        API.disableControlThisFrame(30);
    }
    else {
        API.enableControlThisFrame(30);
    }*/

    //API.sendNotification("Você pressionou: " + keyEventArgs.KeyCode);
    if (keyEventArgs.KeyCode === Keys.F1) 
    {
        API.showCursor(false);
        API.setCanOpenChat(true);

        if (playersBrowser != null) 
        {
             API.destroyCefBrowser(playersBrowser);
             playersBrowser = null;
            return;
         }

         if (banBrowser != null) 
         {
             API.destroyCefBrowser(banBrowser);
             API.triggerServerEvent("kickuser");
             banBrowser = null;
            return;
         }

        if (loginBrowser != null) 
        {
            API.destroyCefBrowser(loginBrowser);
            API.sendNotification("Use /login [usuário] [senha] para autenticação.");
            loginBrowser = null;
            return;
        }

        if (helpBrowser != null) 
        {
             API.destroyCefBrowser(helpBrowser);
             helpBrowser = null;
            return;
         }

         if (statsBrowser != null) 
        {
             API.destroyCefBrowser(statsBrowser);
             statsBrowser = null;
            return;
         }

         if (sosBrowser != null) 
        {
             API.destroyCefBrowser(sosBrowser);
             sosBrowser = null;
            return;
         }

        if (atmBrowser != null) 
        {
             API.destroyCefBrowser(atmBrowser);
             atmBrowser = null;
            return;
         }

         if (conceBrowser != null) 
         {
             API.destroyCefBrowser(conceBrowser);
             API.triggerServerEvent("sairconce");
             conceBrowser = null;
             return;
         }

         if(autoescolaBrowser != null)
         {
             API.destroyCefBrowser(autoescolaBrowser);
             autoescolaBrowser = null;
             return;
         }
    }
    else if (keyEventArgs.KeyCode === Keys.F2 && !API.isChatOpen()) 
    {
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado") || playersBrowser != null) 
            return;

        API.triggerServerEvent("tab_players");    
    }
    else if (keyEventArgs.KeyCode === Keys.F3 && !API.isChatOpen()) {
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado") || playersBrowser != null)
            return;

        API.triggerServerEvent("help_page");
    }
    else if (keyEventArgs.KeyCode === Keys.S && keyEventArgs.Control && !API.isChatOpen()) {
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado") || playersBrowser != null)
            return;

        API.triggerServerEvent("stats_page");
    }
    else if (keyEventArgs.KeyCode === Keys.G && !API.isChatOpen()) 
    {
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado") || !API.isPlayerInAnyVehicle(API.getLocalPlayer()) || API.getPlayerVehicleSeat(API.getLocalPlayer()) != -1) 
            return;

        API.triggerServerEvent("elm");
    }
    else if (keyEventArgs.KeyCode === Keys.Delete && !API.isChatOpen())
    {
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado")) 
            return;

        API.triggerServerEvent("stopanim");
    }
    else if (keyEventArgs.KeyCode === Keys.K && !API.isChatOpen()) {
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado"))
            return;

        API.triggerServerEvent("trancar_destrancar_v");
    }
    else if (keyEventArgs.KeyCode === Keys.Up && !API.isChatOpen()) { //Seta pra cima
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado") || !API.isPlayerInAnyVehicle(API.getLocalPlayer()) || API.getPlayerVehicleSeat(API.getLocalPlayer()) != -1)
            return;

        API.triggerServerEvent("ligar_veiculo");
    }
    else if (keyEventArgs.KeyCode === Keys.Down && !API.isChatOpen()) { //Seta pra baixo
        if (!API.hasEntitySyncedData(API.getLocalPlayer(), "logado") || !API.isPlayerInAnyVehicle(API.getLocalPlayer()) || API.getPlayerVehicleSeat(API.getLocalPlayer()) != -1)
            return;

        API.triggerServerEvent("desligar_veiculo");
    }
    else if(keyEventArgs.KeyCode === Keys.Y)
    {
        if (API.getEntitySyncedData(API.getLocalPlayer(), "CheckpointRoupas") == 1) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAS_CHARACTER_DATA") == true) {
                menu_camisetas.Visible = true;
                itemMenu = 1;
                menu_camisetas.ResetKey(menuControl.Back);
            }
            else API.sendNotification("~r~[ERRO] ~w~Você não tem acesso a este menu.");
        }
        else if (API.getEntitySyncedData(API.getLocalPlayer(), "CheckpointRoupas") == 2) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAS_CHARACTER_DATA") == true) {
                menu_bermudas.Visible = true;
                itemMenu = 3;
                menu_bermudas.ResetKey(menuControl.Back);
            }
            else API.sendNotification("~r~[ERRO] ~w~Você não tem acesso a este menu.");
        }
        else if (API.getEntitySyncedData(API.getLocalPlayer(), "CheckpointRoupas") == 3) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAS_CHARACTER_DATA") == true) {
                menu_calcas.Visible = true;
                itemMenu = 4;
                menu_calcas.ResetKey(menuControl.Back);
            }
            else API.sendNotification("~r~[ERRO] ~w~Você não tem acesso a este menu.");
        }
        else if (API.getEntitySyncedData(API.getLocalPlayer(), "CheckpointRoupas") == 4) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAS_CHARACTER_DATA") == true) {
                menu_casmole.Visible = true;
                itemMenu = 2;
                menu_casmole.ResetKey(menuControl.Back);
            }
            else API.sendNotification("~r~[ERRO] ~w~Você não tem acesso a este menu.");
        }
        else if (API.getEntitySyncedData(API.getLocalPlayer(), "CheckpointRoupas") == 5) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAS_CHARACTER_DATA") == true) {
                menu_calcado.Visible = true;
                itemMenu = 5;
                menu_calcado.ResetKey(menuControl.Back);
            }
            else API.sendNotification("~r~[ERRO] ~w~Você não tem acesso a este menu.");
        }
        else if (API.getEntitySyncedData(API.getLocalPlayer(), "CheckpointBarbearia") == 1) {
            if (API.getEntitySyncedData(API.getLocalPlayer(), "GTAO_HAS_CHARACTER_DATA") == true) {
                menu_barbearia.Visible = true;
                itemMenu = 5;
                menu_barbearia.ResetKey(menuControl.Back);
            }
            else API.sendNotification("~r~[ERRO] ~w~Você não tem acesso a este menu.");
        }
        else
        {
            API.triggerServerEvent("Player_Pressionou_Y");
        }
    }
    if (spamProtection != true) {
        if (keyEventArgs.KeyCode === Keys.Back) {
            if (menu_camisetas.Visible == true || menu_casmole.Visible == true || menu_bermudas.Visible == true || menu_calcas.Visible == true
                || menu_calcado.Visible == true || menu_barbearia.Visible == true || menu_barbas.Visible == true || menu_cabelos.Visible == true) {
                menu_camisetas.Visible = false;
                menu_casmole.Visible = false;
                menu_bermudas.Visible = false;
                menu_calcas.Visible = false;
                menu_calcado.Visible = false;
                menu_barbearia.Visible = false;
                menu_barbas.Visible = false;
                menu_cabelos.Visible = false;
            }
            if (g_menu_confirma.Visible == true) {
                API.triggerServerEvent("Reverter_roupa");
                g_menu_confirma.Visible = false;

                if (itemMenu == 1) {
                    menu_camisetas.Visible = true;
                }
                else if (itemMenu == 2) {
                    menu_casmole.Visible = true;
                }
                else if (itemMenu == 3) {
                    menu_bermudas.Visible = true;
                }
                else if (itemMenu == 4) {
                    menu_calcas.Visible = true;
                }
                else if (itemMenu == 5) {
                    menu_calcado.Visible = true;
                }
            }

            spamProtection = true;
            API.sleep(ANTI_SPAM_TIME);
            spamProtection = false;
        }
        else if (keyEventArgs.KeyCode === Keys.Y)
        {
            if(OnPickupJob > 0){
                API.triggerServerEvent("PressionouEmprego", OnPickupJob);
                spamProtection = true;
                API.sleep(1000);
                spamProtection = false;
            }
            else if (PlayerInMarker > 0) {

                if (PlayerInMarker == 1)
                {
                    if (autoescolaBrowser != null)
                        return;

                    autoescolaBrowser = API.createCefBrowser(res.Width, res.Height);
                    API.setCefBrowserPosition(autoescolaBrowser, 0, 0);
                    API.waitUntilCefBrowserInit(autoescolaBrowser);
                    API.loadPageCefBrowser(autoescolaBrowser, "Client/autoescola.html");
                    API.showCursor(true);
                    API.setCanOpenChat(false);
                    API.sendNotification("Pressione F1 para fechar o dialog.");
                }
                else
                    API.triggerServerEvent("PressedY_Checkpoint", PlayerInMarker);
                spamProtection = true;
                API.sleep(1000);
                spamProtection = false;
            }
        }
    }
});

function ExibirCEF(evento)
{
    if (evento == "players")
    {
        if(playersBrowser != null && param1 != null && param2 != null)
            playersBrowser.call("exibirTabPlayers", param1, param2);
    }
    else if (evento == "sos")
    {
        if(sosBrowser != null && param1 != null && param2 != null)
            sosBrowser.call("exibirTabSOS", param1, param2);
    }
    else if (evento == "stats")
    {
        if(statsBrowser != null && param1 != null)
            statsBrowser.call("exibirStatsPlayer", param1);
    }
    else if (evento == "atm")
    {
        if(atmBrowser != null && param1 != null)
            atmBrowser.call("exibirATM", param1);
    }
    else if (evento == "help")
    {
        if(helpBrowser != null && param1 != null)
            helpBrowser.call("exibirHelp", param1);
    }
    else if (evento == "ban")
    {
        if(banBrowser != null && param1 != null)
            banBrowser.call("exibirBan", param1);
    }
    else if (evento == "conce")
    {
        if(conceBrowser != null && param1 != null)
            conceBrowser.call("exibirConce", param1);
    }
}

function FecharCEF(evento)
{
    API.showCursor(false);
    API.setCanOpenChat(true);

    if(conceBrowser != null)
    {
        API.destroyCefBrowser(conceBrowser);
        conceBrowser = null;
    }
}

function AcaoCEFATM(acao, valor)
{
     if (atmBrowser != null) 
         API.destroyCefBrowser(atmBrowser);

    atmBrowser = null;
    API.showCursor(false);
    API.setCanOpenChat(true);

    API.triggerServerEvent("acaoatm", acao, valor);
}

function logarPlayer(usuario, password)
{
     if (loginBrowser != null) 
         API.destroyCefBrowser(loginBrowser);

    loginBrowser = null;
    API.showCursor(false);
    API.setCanOpenChat(true);

    API.triggerServerEvent("autenticarusuario", usuario, password);
}

function VerVeiculoConce(veh)
{
     if (conceBrowser != null) API.destroyCefBrowser(conceBrowser);

        API.triggerServerEvent("visualizarveiculo", veh);

        conceBrowser = API.createCefBrowser(650, 720);
        const leftPos = res.Width-700;
        const topPos = res.Height-770;
        API.setCefBrowserPosition(conceBrowser, leftPos, topPos);
        API.waitUntilCefBrowserInit(conceBrowser);
        API.loadPageCefBrowser(conceBrowser, "Client/conce2.html"); 
        API.showCursor(true);
        API.setCanOpenChat(false);
        API.sendNotification("Pressione F1 para fechar o dialog.");
}

function AcaoVeiculoConce(acao, r, g, b)
{
    if (acao === "cancelar")
    {
        API.showCursor(false);
        API.setCanOpenChat(true);
         if (conceBrowser != null) 
         {
             API.destroyCefBrowser(conceBrowser);
             API.triggerServerEvent("sairconce");
             conceBrowser = null;
             return;
         }
        return;
    }

    API.triggerServerEvent("acaoveiculoconce", acao, r, g, b);
}

function AutoEscola_IniciarTeste(tipo) // tipos: 1 - carro | 2 - caminhao
{
    API.triggerServerEvent("IniciarTesteAutoEscola", tipo);

    API.showCursor(false);
    API.setCanOpenChat(true);
    API.destroyCefBrowser(autoescolaBrowser);
    autoescolaBrowser = null;
}


var myDog = null;

/* ROUPAS */

/* ==== MASCULINO ====
- Calça: 1
- Bermmuda: 2
- camisa: 3
- Casaco: 4

*/

menu_camisetas.OnItemSelect.connect(function (sender, item, index) {

    API.triggerServerEvent("comprou_roupa", 3, index, 0, 0);

    itemSelecionado = index;
    menu_camisetas.Visible = false;

    g_menu_confirma.Visible = true;
});
menu_calcas.OnItemSelect.connect(function (sender, item, index) {

    API.triggerServerEvent("comprou_roupa", 1, index, 0, 0);

    itemSelecionado = index;
    menu_calcas.Visible = false;

    g_menu_confirma.Visible = true;
});

menu_bermudas.OnItemSelect.connect(function (sender, item, index) {

    API.triggerServerEvent("comprou_roupa", 2, index, 0, 0);

    itemSelecionado = index;
    menu_bermudas.Visible = false;

    g_menu_confirma.Visible = true;
});

menu_casmole.OnItemSelect.connect(function (sender, item, index) {
    API.sendNotification("Experimentando casaco " + index);

    API.triggerServerEvent("comprou_roupa", 4, index, 0, 0);

    texturaEscolhida = 0;
    itemSelecionado = index;
    menu_casmole.Visible = false;

    g_menu_confirma.Visible = true;
});

menu_calcado.OnItemSelect.connect(function (sender, item, index) {
    API.sendNotification("Experimentando calçado " + index);

    API.triggerServerEvent("comprou_roupa", 5, index, 0, 0);

    texturaEscolhida = 0;
    itemSelecionado = index;
    menu_calcado.Visible = false;

    g_menu_confirma.Visible = true;
});
//==============================================================================
g_menu_confirma.OnItemSelect.connect(function (sender, item, index) {

    if (index == 1) {
        API.sendNotification("Você comprou essa roupa, deseja comprar mais algo?");
        g_menu_confirma.Visible = false;
        if (itemMenu == 1) {//camisa
            menu_camisetas.Visible = true;
            API.triggerServerEvent("comprou_roupa", 3, itemSelecionado, 1, texturaEscolhida);
        }
        else if (itemMenu == 2) {//casaco
            menu_casmole.Visible = true;
            API.triggerServerEvent("comprou_roupa", 4, itemSelecionado, 1, texturaEscolhida);
        }
        else if (itemMenu == 3) {//bermuda
            menu_bermudas.Visible = true;
            API.triggerServerEvent("comprou_roupa", 2, itemSelecionado, 1, texturaEscolhida);
        }
        else if (itemMenu == 4) {//Calça
            menu_calcas.Visible = true;
            API.triggerServerEvent("comprou_roupa", 1, itemSelecionado, 1, texturaEscolhida);
        }
        else if (itemMenu == 5) {//Calçado
            menu_calcado.Visible = true;
            API.triggerServerEvent("comprou_roupa", 5, itemSelecionado, 1, texturaEscolhida);
        }

    }
    else if (index == 2) {
        API.sendNotification("Retornando ao menu");
        API.triggerServerEvent("Reverter_roupa");

        g_menu_confirma.Visible = false;
        if (itemMenu == 1) {
            menu_camisetas.Visible = true;
        }
        else if (itemMenu == 2) {
            menu_casmole.Visible = true;
        }
        else if (itemMenu == 3) {
            menu_bermudas.Visible = true;
        }
        else if (itemMenu == 4) {
            menu_calcas.Visible = true;
        }
        else if (itemMenu == 5) {
            menu_calcado.Visible = true;
        }

        //Reabrir menu
        itemMenu = 0;
    }
});

/*
Texturas ( TIPOS )
1 - Camisas
2 - Casacos / Moletos
3 - Bermudas
4 - Calça
5 - Calçado
*/
function TexturaValida_Camisa(tipo, item, index) {
    var retorno = false;
    if (tipo == 1) {
        switch (item) {
            case 0: if (index == 0 || index == 1 || index == 3 || index == 4 || index == 5 || index == 6 || index == 7 || index == 8 || index == 11 || index == 12 || index == 14) retorno = true; break;
            case 1: if (index == 0 || index == 1 || index == 7) retorno = true; break;
            case 2: if (index == 0 || index == 10 || index == 13 || index == 14) retorno = true; break;
            case 3: if ((index >= 0 && index <= 7) || (index >= 10 && index <= 15)) retorno = true; break;
            case 4: if ((index >= 0 && index <= 11)) retorno = true; break;

            case 5: if ((index >= 0 && index <= 5 && index != 4) || index == 13) retorno = true; break;

            case 6: if ((index >= 0 && index <= 15)) retorno = true; break;
            case 7: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 8: if ((index >= 0 && index <= 5)) retorno = true; break;
            case 9: if ((index >= 0 && index <= 3)) retorno = true; break;
            case 10: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 11: if ((index >= 0 && index <= 2)) retorno = true; break;

            case 12: if ((index >= 0 && index <= 9)) retorno = true; break;
            case 13: if (index == 0) retorno = true; break;

            case 14: if ((index >= 0 && index <= 1)) retorno = true; break;
            case 15: if ((index >= 0 && index <= 5)) retorno = true; break;
            case 16: if ((index >= 0 && index <= 4)) retorno = true; break;
            case 17: if ((index >= 0 && index <= 1)) retorno = true; break;

            case 18: if ((index >= 0 && index <= 3)) retorno = true; break;
            case 19: if (index == 0) retorno = true; break;
            case 20: if (index == 0) retorno = true; break;
            case 21: if ((index >= 0 && index <= 3)) retorno = true; break;

            case 22: if ((index >= 0 && index <= 1)) retorno = true; break;
            case 23: if ((index >= 0 && index <= 3)) retorno = true; break;
            case 24: if (index == 0) retorno = true; break;
            case 25: if (index == 0) retorno = true; break;

            case 26: if ((index >= 0 && index <= 3)) retorno = true; break;
            case 27: if (index == 0) retorno = true; break;
            case 28: if ((index >= 0 && index <= 16)) retorno = true; break;
            case 29: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 30: if ((index >= 0 && index <= 2)) retorno = true; break;

            case 31: if ((index >= 0 && index <= 15)) retorno = true; break;
            case 32: if ((index >= 0 && index <= 4)) retorno = true; break;
            case 33: if (index == 0) retorno = true; break;
            case 34: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 35: if ((index >= 0 && index <= 2)) retorno = true; break;

            case 36: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 37: if ((index >= 0 && index <= 1)) retorno = true; break;
            case 38: if ((index >= 0 && index <= 1)) retorno = true; break;
            case 39: if (index == 0) retorno = true; break;
            case 40: if ((index >= 0 && index <= 4)) retorno = true; break;

            case 41: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 42: if ((index >= 0 && index <= 14)) retorno = true; break;
            case 43: if ((index >= 0 && index <= 9)) retorno = true; break;
            case 44: if (index == 0) retorno = true; break;
            case 45: if (index == 0) retorno = true; break;

            case 46: if (index == 0) retorno = true; break;
            case 47: if ((index >= 0 && index <= 6)) retorno = true; break;
            case 48: if ((index >= 0 && index <= 8)) retorno = true; break;
            case 49: if ((index >= 0 && index <= 11)) retorno = true; break;
            case 50: if ((index >= 0 && index <= 15)) retorno = true; break;

            case 51: if ((index >= 0 && index <= 10)) retorno = true; break;
            case 52: if ((index >= 0 && index <= 16)) retorno = true; break;
            case 53: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 54: if ((index >= 0 && index <= 15)) retorno = true; break;
            case 55: if ((index >= 0 && index <= 2)) retorno = true; break;
            case 56: if ((index >= 0 && index <= 4)) retorno = true; break;
            case 57: if ((index >= 0 && index <= 4)) retorno = true; break;
            case 58: if ((index >= 0 && index <= 2)) retorno = true; break;

            default:
                retorno = true;
                break;
        }
    }
    else if (tipo == 2) {
        switch (item) {
            case 0: if (index == 0) retorno = true; break;
            case 1: if (index >= 0 && index <= 4) retorno = true; break;
            case 2: if (index == 0) retorno = true; break;
            case 3: if (index == 0) retorno = true; break;
            case 4: if (index >= 0 && index <= 3) retorno = true; break;
            case 5: if (index >= 0 && index <= 10) retorno = true; break;
            case 6: if (index >= 0 && index <= 15) retorno = true; break;
            case 7: if (index == 0) retorno = true; break;
            case 8: if (index >= 0 && index <= 5) retorno = true; break;
            case 9: if (index >= 0 && index <= 4) retorno = true; break;
            case 10: if (index >= 0 && index <= 11) retorno = true; break;
            case 11: if (index >= 0 && index <= 3) retorno = true; break;
            case 12: if (index == 0) retorno = true; break;

            case 13: if (index == 0) retorno = true; break;
            case 14: if (index == 0) retorno = true; break;
            case 15: if (index >= 0 && index <= 5) retorno = true; break;
            case 16: if (index >= 0 && index <= 11) retorno = true; break;
            case 17: if (index == 0) retorno = true; break;
            case 18: if (index == 0) retorno = true; break;
            case 19: if (index >= 0 && index <= 2) retorno = true; break;
            case 20: if (index >= 0 && index <= 9) retorno = true; break;

            case 21: if (index >= 0 && index <= 9) retorno = true; break;
            case 22: if (index >= 0 && index <= 11) retorno = true; break;
            case 23: if (index >= 0 && index <= 16) retorno = true; break;
            case 24: if (index >= 0 && index <= 7) retorno = true; break;
            case 25: if (index >= 0 && index <= 1) retorno = true; break;
            case 26: if (index >= 0 && index <= 1) retorno = true; break;
            case 27: if (index >= 0 && index <= 3) retorno = true; break;
            case 28: if (index >= 0 && index <= 12) retorno = true; break;
            case 29: if (index >= 0 && index <= 10) retorno = true; break;
            case 30: if (index >= 0 && index <= 16) retorno = true; break;

            case 31: if (index >= 0 && index <= 16) retorno = true; break;
            case 32: if (index >= 0 && index <= 16) retorno = true; break;
            case 33: if (index >= 0 && index <= 12) retorno = true; break;
            case 34: if (index >= 0 && index <= 0) retorno = true; break;
            default:
                retorno = true;
                break;
        }
    }
    else { retorno = true; }

    return retorno;
}

TextureMenu.OnListChanged.connect(function (sender, new_index) {

    //==================================================
    if (itemMenu == 1) // Camisas
    {
        if (TexturaValida_Camisa(itemMenu, itemSelecionado, new_index) == true) {
            API.triggerServerEvent("comprou_roupa", 3, itemSelecionado, 0, new_index);
            texturaEscolhida = new_index;
        }
        else {
            API.triggerServerEvent("comprou_roupa", 3, itemSelecionado, 0, 0);
            texturaEscolhida = 0;
        }
    }
    else if (itemMenu == 2) // Casacos - Moletons
    {
        if (TexturaValida_Camisa(itemMenu, itemSelecionado, new_index) == true) {

            API.triggerServerEvent("comprou_roupa", 4, itemSelecionado, 0, new_index);
            texturaEscolhida = new_index;
        }
        else {
            API.triggerServerEvent("comprou_roupa", 4, itemSelecionado, 0, 0);
            texturaEscolhida = 0;
        }
    }
    else if (itemMenu == 3) // Bermudas
    {
        if (TexturaValida_Camisa(itemMenu, itemSelecionado, new_index) == true) {
            API.triggerServerEvent("comprou_roupa", 2, itemSelecionado, 0, new_index);
            texturaEscolhida = new_index;
        }
        else {
            API.triggerServerEvent("comprou_roupa", 2, itemSelecionado, 0, 0);
            texturaEscolhida = 0;
        }
    }
    else if (itemMenu == 4) // Calcas
    {
        if (TexturaValida_Camisa(itemMenu, itemSelecionado, new_index) == true) {
            API.triggerServerEvent("comprou_roupa", 1, itemSelecionado, 0, new_index);
            texturaEscolhida = new_index;
        }
        else {
            API.triggerServerEvent("comprou_roupa", 1, itemSelecionado, 0, 0);
            texturaEscolhida = 0;
        }
    }
    else if (itemMenu == 5) // Calçados
    {
        if (TexturaValida_Camisa(itemMenu, itemSelecionado, new_index) == true) {
            API.triggerServerEvent("comprou_roupa", 5, itemSelecionado, 0, new_index);
            texturaEscolhida = new_index;
        }
        else {
            API.triggerServerEvent("comprou_roupa", 5, itemSelecionado, 0, 0);
            texturaEscolhida = 0;
        }
    }
    //==================================================
});

function changeGender(x) {
    if (x == 2) // female
        API.triggerServerEvent("Criar_Pers_M");
    else //Male
        API.triggerServerEvent("Criar_Pers_H");
}

function closeCreationMenu() {
    API.showCursor(false);
    API.destroyCefBrowser(creatercharBrowser);
    creatercharBrowser = null;

    API.triggerServerEvent("CriouPersonagem");
}

function Change_Player_Skin(parte, new_index) {

    switch (parte) {
        case 0: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SHAPE_FIRST_ID", new_index); break;
        case 1: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SHAPE_SECOND_ID", new_index); break;
        case 2: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SKIN_FIRST_ID", new_index); break;
        case 3: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SKIN_SECOND_ID", new_index); break;
        case 4: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SHAPE_MIX", new_index); break;
        case 5: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SKIN_MIX", new_index); break;
        case 6: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_COLOR", new_index); break;
        case 7: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_HIGHLIGHT_COLOR", new_index); break;
        case 8: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYE_COLOR", new_index); break;
        case 9: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYEBROWS_COLOR", new_index); break;
        case 10: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_MAKEUP_COLOR", new_index); break;
        case 11: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_LIPSTICK_COLOR", new_index); break;
        case 12: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYEBROWS_COLOR2", new_index); break;
        case 13: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_MAKEUP_COLOR2", new_index); break;
        case 14: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_LIPSTICK_COLOR2", new_index); break;
        case 15: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_STYLE", new_index); break;
        case 16: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_1", new_index); break;
        case 17: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_2", new_index); break;
        case 18: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_3", new_index); break;
        case 19: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_4", new_index); break;
        case 20: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_5", new_index); break;
        case 21: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_6", new_index); break;
        case 22: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_7", new_index); break;
        case 23: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_8", new_index); break;
        case 24: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_9", new_index); break;
        case 25: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_10", new_index); break;
        case 26: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_11", new_index); break;
        case 27: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_12", new_index); break;
        case 28: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_13", new_index); break;
        case 29: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_14", new_index); break;
        case 30: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_15", new_index); break;
        case 31: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_16", new_index); break;
        case 32: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_17", new_index); break;
        case 33: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_18", new_index); break;
        case 34: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_19", new_index); break;
        case 35: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_20", new_index); break;
        case 36: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_21", new_index); break;
        case 37: API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYEBROWS", new_index); break;

    }
    API.triggerServerEvent("Criando_Pers_Update", parte, new_index);
}

//===============================================================================================
//                  CRIANDO PERSONAGEM CUSTOM
//===============================================================================================
API.onEntityStreamIn.connect(function (ent, entType) {
    if (entType == 6 || entType == 8) {// Player or ped
        setPedCharacter(ent);
    }
});
API.onResourceStart.connect(function () {
    var players = API.getStreamedPlayers();

    for (var i = players.Length - 1; i >= 0; i--) {
        setPedCharacter(players[i]);
    }

    API.setShowWastedScreenOnDeath(false);
});

function setPedCharacter(ent) {
    if (API.getEntitySyncedData(ent, "GTAO_HAS_CHARACTER_DATA") === true) {
        // FACE
        var shapeFirstId = API.getEntitySyncedData(ent, "GTAO_SHAPE_FIRST_ID");
        var shapeSecondId = API.getEntitySyncedData(ent, "GTAO_SHAPE_SECOND_ID");

        var skinFirstId = API.getEntitySyncedData(ent, "GTAO_SKIN_FIRST_ID");
        var skinSecondId = API.getEntitySyncedData(ent, "GTAO_SKIN_SECOND_ID");

        var shapeMix = API.f(API.getEntitySyncedData(ent, "GTAO_SHAPE_MIX"));
        var skinMix = API.f(API.getEntitySyncedData(ent, "GTAO_SKIN_MIX"));

        API.callNative("SET_PED_HEAD_BLEND_DATA", ent, shapeFirstId, shapeSecondId, 0, skinFirstId, skinSecondId, 0, shapeMix, skinMix, 0, false);

        // HAIR COLOR - Style
        var hairStyle = API.getEntitySyncedData(ent, "GTAO_HAIR_STYLE");

        var hairColor = API.getEntitySyncedData(ent, "GTAO_HAIR_COLOR");
        var highlightColor = API.getEntitySyncedData(ent, "GTAO_HAIR_HIGHLIGHT_COLOR");

        API.setPlayerClothes(ent, 2, hairStyle, 0);
        API.callNative("_SET_PED_HAIR_COLOR", ent, hairColor, highlightColor);

        // EYE COLOR

        var eyeColor = API.getEntitySyncedData(ent, "GTAO_EYE_COLOR");

        API.callNative("_SET_PED_EYE_COLOR", ent, eyeColor);

        // EYEBROWS, MAKEUP, LIPSTICK
        var eyebrowsStyle = API.getEntitySyncedData(ent, "GTAO_EYEBROWS");
        var eyebrowsColor = API.getEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR");
        var eyebrowsColor2 = API.getEntitySyncedData(ent, "GTAO_EYEBROWS_COLOR2");

        API.callNative("SET_PED_HEAD_OVERLAY", ent, 2, eyebrowsStyle, API.f(1));

        API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 2, 1, eyebrowsColor, eyebrowsColor2);

        if (API.hasEntitySyncedData(ent, "GTAO_LIPSTICK")) {
            var lipstick = API.getEntitySyncedData(ent, "GTAO_LIPSTICK");
            var lipstickColor = API.getEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR");
            var lipstickColor2 = API.getEntitySyncedData(ent, "GTAO_LIPSTICK_COLOR2");

            API.callNative("SET_PED_HEAD_OVERLAY", ent, 8, lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 8, 2, lipstickColor, lipstickColor2);
        }

        if (API.hasEntitySyncedData(ent, "GTAO_MAKEUP")) {
            var makeup = API.getEntitySyncedData(ent, "GTAO_MAKEUP");
            var makeupColor = API.getEntitySyncedData(ent, "GTAO_MAKEUP_COLOR");
            var makeupColor2 = API.getEntitySyncedData(ent, "GTAO_MAKEUP_COLOR2");

            API.callNative("SET_PED_HEAD_OVERLAY", ent, 4, makeup, API.f(1));
            API.callNative("SET_PED_HEAD_OVERLAY", ent, 8, lipstick, API.f(1));
            API.callNative("_SET_PED_HEAD_OVERLAY_COLOR", ent, 4, 0, makeupColor, makeupColor2);
        }

        // FACE FEATURES (e.g. nose length, chin shape, etc)

        var faceFeatureList = API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_LIST");

        for (var i = 0; i < 21; i++) {
            if (i == 0) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_1"));
            else if (i == 1) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_2"));
            else if (i == 2) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_3"));
            else if (i == 3) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_4"));
            else if (i == 4) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_5"));
            else if (i == 5) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_6"));
            else if (i == 6) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_7"));
            else if (i == 7) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_8"));
            else if (i == 8) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_9"));
            else if (i == 9) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_10"));
            else if (i == 10) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_11"));
            else if (i == 11) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_12"));
            else if (i == 12) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_13"));
            else if (i == 13) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_14"));
            else if (i == 14) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_15"));
            else if (i == 15) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_16"));
            else if (i == 16) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_17"));
            else if (i == 17) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_18"));
            else if (i == 18) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_19"));
            else if (i == 19) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_20"));
            else if (i == 20) API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_21"));

            else API.callNative("_SET_PED_FACE_FEATURE", ent, i, API.f(faceFeatureList[i]));
            // API.sendNotification(i + " - " + API.getEntitySyncedData(ent, "GTAO_FACE_FEATURES_16"));
        }

    }
    /*else
        API.sendNotification("Deu ruim 'Freemode-#0001'");*/
}

/* Menu Criar Personagem */
var GenderMenu = API.createMenu("Personagem", "Crie o seu personagem personalizado..", 0, 0, 6);
GenderMenu.ResetKey(menuControl.Back);
GenderMenu.AddItem(API.createMenuItem("Masculino", "Selecionar o sexo"));
GenderMenu.AddItem(API.createMenuItem("Feminino", "Selecionar o sexo"));

GenderMenu.OnItemSelect.connect(function (sender, item, index) {

    if (index == 0) {
        API.triggerServerEvent("Criar_Pers_H");
    }
    else {
        API.triggerServerEvent("Criar_Pers_M");
    }

    GenderMenu.Visible = false;
    criar_pers.Visible = true;
});

var criar_pers = API.createMenu("Personagem", "Crie o seu personagem personalizado..", 0, 0, 6);
criar_pers.ResetKey(menuControl.Back);

var list_textura = new List(String);
list_textura.Add("0");
list_textura.Add("1");
list_textura.Add("2");
list_textura.Add("3");
list_textura.Add("4");
list_textura.Add("5");
list_textura.Add("6");
list_textura.Add("7");
list_textura.Add("8");
list_textura.Add("9");
list_textura.Add("10");
list_textura.Add("11");
list_textura.Add("12");
list_textura.Add("13");
list_textura.Add("14");
list_textura.Add("15");
list_textura.Add("16");
list_textura.Add("17");
list_textura.Add("18");
list_textura.Add("19");
list_textura.Add("20");
list_textura.Add("21");
list_textura.Add("22");

list_textura.Add("23");
list_textura.Add("24");
list_textura.Add("25");
list_textura.Add("26");
list_textura.Add("27");
list_textura.Add("28");
list_textura.Add("29");
list_textura.Add("30");
list_textura.Add("31");
list_textura.Add("32");
list_textura.Add("33");
list_textura.Add("34");
list_textura.Add("35");
list_textura.Add("36");
list_textura.Add("37");
list_textura.Add("38");
list_textura.Add("39");
list_textura.Add("40");
list_textura.Add("41");
list_textura.Add("42");
list_textura.Add("43");
list_textura.Add("44");
list_textura.Add("45");

var list_mix = new List(String);
list_mix.Add("0");
list_mix.Add("0.1");
list_mix.Add("0.2");
list_mix.Add("0.3");
list_mix.Add("0.4");
list_mix.Add("0.5");
list_mix.Add("0.6");
list_mix.Add("0.7");
list_mix.Add("0.8");
list_mix.Add("0.9");
list_mix.Add("1.0");

var ped_menu_1 = API.createListItem("Rosto 1", "Selecione", list_textura, 0);
var ped_menu_2 = API.createListItem("Rosto 2", "Selecione", list_textura, 0);
var ped_menu_3 = API.createListItem("Cor da Pele", "Selecione", list_textura, 0);
var ped_menu_4 = API.createListItem("Cor da Pele 2", "Selecione", list_textura, 0);

var ped_menu_5 = API.createListItem("Mixar Rostos", "Selecione", list_mix, 0);
var ped_menu_6 = API.createListItem("Mixar Peles", "Selecione", list_mix, 0);

var ped_menu_cabelo = API.createListItem("Cabelo", "Selecione", list_textura, 0);
var ped_menu_7 = API.createListItem("Cor do Cabelo", "Selecione", list_textura, 0);
var ped_menu_8 = API.createListItem("Hair Light", "Selecione", list_textura, 0);
var ped_menu_olhos = API.createListItem("Sobrancelhas", "Selecione", list_textura, 0);
var ped_menu_olhos_c = API.createListItem("Cor das Sobrancelhas", "Selecione", list_textura, 0);
var ped_menu_cor_olhos = API.createListItem("Cor dos olhos", "Selecione", list_textura, 0);

var list_rostoDetail = new List(String);
list_rostoDetail.Add("0");
list_rostoDetail.Add("0.2");
list_rostoDetail.Add("0.4");
list_rostoDetail.Add("0.6");
list_rostoDetail.Add("1");
list_rostoDetail.Add("-1");
list_rostoDetail.Add("-0.6");
list_rostoDetail.Add("-0.4");
list_rostoDetail.Add("-0.2");
var ped_menu_face_1 = API.createListItem("Largura do Nariz", "Selecione", list_rostoDetail, 0);
var ped_menu_face_2 = API.createListItem("Altura do Nariz", "Selecione", list_rostoDetail, 0);
var ped_menu_face_3 = API.createListItem("Tamanho do Nariz", "Selecione", list_rostoDetail, 0);
var ped_menu_face_4 = API.createListItem("Altura Nariz (Em cima)", "Selecione", list_rostoDetail, 0);
var ped_menu_face_5 = API.createListItem("Inclinação do Nariz", "Selecione", list_rostoDetail, 0);
var ped_menu_face_6 = API.createListItem("Nariz Torto", "Selecione", list_rostoDetail, 0);
var ped_menu_face_7 = API.createListItem("Altura da Sobrancelha", "Selecione", list_rostoDetail, 0);
var ped_menu_face_8 = API.createListItem("Altura da Sobrancelha", "Selecione", list_rostoDetail, 0);
var ped_menu_face_9 = API.createListItem("Bocheca", "Selecione", list_rostoDetail, 0);
var ped_menu_face_10 = API.createListItem("Altura da Bochecha", "Selecione", list_rostoDetail, 0);
var ped_menu_face_11 = API.createListItem("Altura da bocheca (baixo)", "Selecione", list_rostoDetail, 0);
var ped_menu_face_12 = API.createListItem("Tamanho dos Olhos", "Selecione", list_rostoDetail, 0);
var ped_menu_face_13 = API.createListItem("Tamanho da boca", "Selecione", list_rostoDetail, 0);
var ped_menu_face_14 = API.createListItem("Tamanho do Maxilar", "Selecione", list_rostoDetail, 0);
var ped_menu_face_15 = API.createListItem("Tamanho do Maxilar 2", "Selecione", list_rostoDetail, 0);
var ped_menu_face_16 = API.createListItem("Formato do Queixo", "Selecione", list_rostoDetail, 0);

var ped_menu_face_17 = API.createListItem("Tamanho do Queixo", "Selecione", list_rostoDetail, 0);
var ped_menu_face_18 = API.createListItem("Largura do Queixo", "Selecione", list_rostoDetail, 0);
var ped_menu_face_19 = API.createListItem("Detalhe no Queixo", "Selecione", list_rostoDetail, 0);
var ped_menu_face_20 = API.createListItem("Largura do Pescoço", "Selecione", list_rostoDetail, 0);
var ped_menu_face_21 = API.createListItem("Ped Face 20", "Selecione", list_rostoDetail, 0);

criar_pers.AddItem(API.createMenuItem("FINALIZAR", "Finalizar edição"));
criar_pers.AddItem(ped_menu_1);
criar_pers.AddItem(ped_menu_2);
criar_pers.AddItem(ped_menu_3);
criar_pers.AddItem(ped_menu_4);
criar_pers.AddItem(ped_menu_5);
criar_pers.AddItem(ped_menu_6);
criar_pers.AddItem(ped_menu_cabelo);
criar_pers.AddItem(ped_menu_7);
criar_pers.AddItem(ped_menu_8);
criar_pers.AddItem(ped_menu_olhos);
criar_pers.AddItem(ped_menu_olhos_c);
criar_pers.AddItem(ped_menu_cor_olhos);
criar_pers.AddItem(ped_menu_face_1);
criar_pers.AddItem(ped_menu_face_2);
criar_pers.AddItem(ped_menu_face_3);
criar_pers.AddItem(ped_menu_face_4);
criar_pers.AddItem(ped_menu_face_5);
criar_pers.AddItem(ped_menu_face_6);
criar_pers.AddItem(ped_menu_face_7);
criar_pers.AddItem(ped_menu_face_8);
criar_pers.AddItem(ped_menu_face_9);
criar_pers.AddItem(ped_menu_face_10);
criar_pers.AddItem(ped_menu_face_11);
criar_pers.AddItem(ped_menu_face_12);
criar_pers.AddItem(ped_menu_face_13);
criar_pers.AddItem(ped_menu_face_14);
criar_pers.AddItem(ped_menu_face_15);
criar_pers.AddItem(ped_menu_face_16);
criar_pers.AddItem(ped_menu_face_17);
criar_pers.AddItem(ped_menu_face_18);
criar_pers.AddItem(ped_menu_face_19);
criar_pers.AddItem(ped_menu_face_20);
criar_pers.AddItem(ped_menu_face_21);

criar_pers.OnItemSelect.connect(function (sender, item, index) {

    if (index == 0) {
        criar_pers.Visible = false;
        API.triggerServerEvent("CriouPersonagem");
    }
});

ped_menu_1.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SHAPE_FIRST_ID", new_index);
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(0, new_index);
});
ped_menu_2.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SHAPE_SECOND_ID", new_index);
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(1, new_index);
});
ped_menu_3.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SKIN_FIRST_ID", new_index);
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(2, new_index);
});
ped_menu_4.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SKIN_SECOND_ID", new_index);
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(3, new_index);
});
ped_menu_5.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SHAPE_MIX", GetPedMix(new_index));
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(4, GetPedMix(new_index));
});
ped_menu_6.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_SKIN_MIX", GetPedMix(new_index));
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(5, GetPedMix(new_index));
});
//
ped_menu_7.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_COLOR", new_index);
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(6, new_index);
});
ped_menu_8.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_HIGHLIGHT_COLOR", new_index);
    setPedCharacter(API.getLocalPlayer());
    CreateMenu_CamPos(0);
    Change_Player_Skin(7, new_index);
});

ped_menu_face_1.OnListChanged.connect(function (sender, new_index) {

    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_1", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(16, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_2.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_2", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(17, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_3.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_3", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(18, GetPedFace(new_index, 0));

    CreateMenu_CamPos(1);
});
ped_menu_face_4.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_4", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(19, GetPedFace(new_index, 0));

    CreateMenu_CamPos(1);
});
ped_menu_face_5.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_5", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(20, GetPedFace(new_index, 0));

    CreateMenu_CamPos(1);
});
ped_menu_face_6.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_6", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(21, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_7.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_7", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(22, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_8.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_8", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(23, GetPedFace(new_index, 0));

    CreateMenu_CamPos(1);
});
ped_menu_face_9.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_9", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(24, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_10.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_10", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(25, GetPedFace(new_index, 0));

    CreateMenu_CamPos(1);
});
ped_menu_face_11.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_11", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(26, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_12.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_12", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(27, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_13.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_13", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(28, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_14.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_14", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(29, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_15.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_15", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(30, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_16.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_16", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(31, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_17.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_17", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(32, GetPedFace(new_index, 0));

    CreateMenu_CamPos(1);
});
ped_menu_face_18.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_18", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(33, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_19.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_19", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(34, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_20.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_20", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(35, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});
ped_menu_face_21.OnListChanged.connect(function (sender, new_index) {
    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_FACE_FEATURES_21", GetPedFace(new_index));
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(36, GetPedFace(new_index, 0));

    CreateMenu_CamPos(2);
});

ped_menu_cabelo.OnListChanged.connect(function (sender, new_index) {

    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_HAIR_STYLE", new_index);
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(15, new_index);

    CreateMenu_CamPos(0);
});

ped_menu_olhos.OnListChanged.connect(function (sender, new_index) {

    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYEBROWS", new_index);
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(37, new_index);

    CreateMenu_CamPos(2);
});
ped_menu_olhos_c.OnListChanged.connect(function (sender, new_index) {

    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYEBROWS_COLOR", new_index);
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(9, new_index);

    CreateMenu_CamPos(2);
});
ped_menu_cor_olhos.OnListChanged.connect(function (sender, new_index) {

    API.setEntitySyncedData(API.getLocalPlayer(), "GTAO_EYE_COLOR", new_index);
    setPedCharacter(API.getLocalPlayer());
    Change_Player_Skin(8, new_index);

    CreateMenu_CamPos(2);
});

function GetPedMix(index)
{
    var retorno;
    if (index == 0) retorno = 0.0;
    else if (index == 1) retorno = 0.1;
    else if (index == 2) retorno = 0.2;
    else if (index == 3) retorno = 0.3;
    else if (index == 4) retorno = 0.4;
    else if (index == 5) retorno = 0.5;
    else if (index == 6) retorno = 0.6;
    else if (index == 7) retorno = 0.7;
    else if (index == 8) retorno = 0.8;
    else if (index == 9) retorno = 0.9;
    else if (index == 10) retorno = 1.0;
    return retorno;
}

function GetPedFace(index, part) {

    if (part == 1) CreateMenu_CamPos(1); //Rosto Lateral
    else CreateMenu_CamPos(2); //Rosto de Frente

    var retorno;
    if (index == 0) retorno = 0;
    else if (index == 1) retorno = 0.2;
    else if (index == 2) retorno = 0.4;
    else if (index == 3) retorno = 0.6;
    else if (index == 4) retorno = 0.9;
    else if (index == 5) retorno = -0.9;
    else if (index == 6) retorno = -0.6;
    else if (index == 7) retorno = -0.4;
    else if (index == 8) retorno = -0.2;
    return retorno;
}

function CreateMenu_CamPos(pos)
{
    if (pos == 1) { //Rosto Lateral
        var newCam = API.createCamera(new Vector3(402.4244, -997.091, -98.50404), new Vector3(400.9244, -997.491, -98.00404));
        API.pointCameraAtPosition(newCam, new Vector3(402.9244, -996.588, -98.50025));
        API.setActiveCamera(newCam);
    }
    else if (pos == 2) { //Rosto de Frente
        var newCam = API.createCamera(new Vector3(402.9244, -997.491, -98.50404), new Vector3(400.9244, -997.491, -98.00404));
        API.pointCameraAtPosition(newCam, new Vector3(402.9244, -996.288, -98.50025));
        API.setActiveCamera(newCam);
    }
    else //Corpo de Frente
    {
        var newCam = API.createCamera(new Vector3(402.9244, -999.491, -99.00404), new Vector3(402.9244, -999.491, -99.00404));
        API.pointCameraAtPosition(newCam, new Vector3(402.9244, -996.288, -99.00025));
        API.setActiveCamera(newCam);
    }
}

API.onLocalPlayerDamaged.connect(function (enemy, weapon, bone) {
    if (API.getPlayerHealth(API.getLocalPlayer()) < 15)
    {
        API.setPlayerHealth(15);
        API.triggerServerEvent("PlayerAsDead");
    }
})


//==================
//---- [BARBEARIA]
var menu_barbearia = API.createMenu("Barbearia", "Selecione uma opção", 0, 0, 6);
menu_barbearia.AddItem(API.createMenuItem("Cabelo", "Cortar"));
menu_barbearia.AddItem(API.createMenuItem("Barba", "Cortar"));

var lista_cabelos = new List(String);
lista_cabelos.Add("0");
lista_cabelos.Add("1");
lista_cabelos.Add("2");
lista_cabelos.Add("3");
lista_cabelos.Add("4");
lista_cabelos.Add("5");
lista_cabelos.Add("6");
lista_cabelos.Add("7");
lista_cabelos.Add("8");
lista_cabelos.Add("9");
lista_cabelos.Add("10");
lista_cabelos.Add("11");
lista_cabelos.Add("12");
lista_cabelos.Add("13");
lista_cabelos.Add("14");
lista_cabelos.Add("15");
lista_cabelos.Add("16");
lista_cabelos.Add("17");
lista_cabelos.Add("18");
lista_cabelos.Add("19");
lista_cabelos.Add("20");
lista_cabelos.Add("21");
lista_cabelos.Add("22");
lista_cabelos.Add("23");
lista_cabelos.Add("24");
lista_cabelos.Add("25");
lista_cabelos.Add("26");
lista_cabelos.Add("27");
lista_cabelos.Add("28");
var menu_cabelos = API.createListItem("Cabelos", "Selecione", lista_cabelos, 0);
var menu_barbas = API.createListItem("Barbas", "Selecione", lista_cabelos, 0);

menu_barbearia.OnItemSelect.connect(function (sender, item, index) {
    switch(item)
    {
        case 0:
            menu_cabelos.Visible = true;
            break;
        case 1:
            menu_barbas.Visible = true;
            break;
    }
    API.showCursor(false);
    menu_barbearia.Visible = false;
});

menu_barbas.OnListChanged.connect(function (sender, new_index) {
    API.setPlayerHeadOverlay(API.getLocalPlayer(), 1, new_index, 1);
});

menu_cabelos.OnListChanged.connect(function (sender, new_index) {
    API.setPlayerHairStyle(API.getLocalPlayer(), new_index, 0, 0, 0, 1);
});


//=============================================================================
var CountLSS;
var Porcentagem;
function LosSantosService(){
    Porcentagem = 0;
    CountLSS = API.every(2000, LosSantosService_F);
}

function LosSantosService_F(){
    API.drawText(Porcentagem+"/100", (res.Width / 2) - 302.4000244140625, (res.Height / 2) - 66.60000610351562, 0.800000011920929, 242, 245, 242, 255, 2, 1, false, false, 0);
    Porcentagem += 10;

    if(Porcentagem == 100)
        API.stop(CountLSS);
}

