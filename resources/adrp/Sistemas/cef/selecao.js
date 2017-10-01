var face_one = 0;
var	face_two = 0;

var skin_mix = 0;
var face_mix = 0;

var skin_one = 0;
var skin_two = 0;

var hair_style = 0;
var hair_color = 0;
var hair_light = 0;

function Genero(x) {
    if(x == 2) // female
		resourceCall("changeGender", 1);
	else //Male
		resourceCall("changeGender", 2);
}

function ShapeMix_Face(soma) {
    if(soma == 1)
		face_mix++;
	else
		face_mix--;
	
	resourceCall("Change_Player_Skin", 0, face_mix);
}

function Shape_Face_one(soma) {
    if(soma == 1)
		face_one++;
	else
		face_one--;
	
	resourceCall("Change_Player_Skin", 1, face_one);
}
function Shape_Face_two(soma) {
    if(soma == 1)
		face_two++;
	else
		face_two--;
	
	resourceCall("Change_Player_Skin", 2, face_two);
}
function Skin_mix(soma) {
    if(soma == 1)
		skin_mix++;
	else
		skin_mix--;
	
	resourceCall("Change_Player_Skin", 3, skin_mix);
}
function Skin_one(soma) {
    if(soma == 1)
		skin_one++;
	else
		skin_one--;
	
	resourceCall("Change_Player_Skin", 4, skin_one);
}
function Skin_two(soma) {
    if(soma == 1)
		skin_two++;
	else
		skin_two--;
	
	if(skin_two < 0) skin_two = 1;
	if(skin_two > 45) skin_two = 45; 
	
	resourceCall("Change_Player_Skin", 5, skin_two);
}
function Cabelo_Style(soma) {
    if(soma == 1)
		hair_style++;
	else
		hair_style--;
	
	if(hair_style >= 0 || hair_style <= 45)
		resourceCall("Change_Player_Skin", 6, hair_style);
}

function Hair_Color(soma) {
    if(soma == 1)
		hair_color++;
	else
		hair_color--;
	
	if(hair_style >= 0 || hair_style <= 45)
		resourceCall("Change_Player_Skin", 7, hair_color);
}

function Hair_Light(soma) {
    if(soma == 1)
		hair_light++;
	else
		hair_light--;
	
	if(hair_style >= 0 || hair_style <= 45)
		resourceCall("Change_Player_Skin", 8, hair_light);
}

function FecharMenuCriacao() {
	resourceCall("closeCreationMenu");
}
