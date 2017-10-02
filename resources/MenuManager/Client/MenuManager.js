var menu = null;
var menuItems = null;

API.onServerEventTrigger.connect(function (name, args) {
    if (name === "MenuManager_OpenMenu") {
        menuItems = new Array();
        let menuData = JSON.parse(args[0]);

        if (menuData.Id === undefined) {
            menuData.Id = "";
        }
        if (menuData.Title === undefined || menuData.Title === "") {
            menuData.Title = " ";
        }

        if (menuData.SubTitle === undefined || menuData.SubTitle === "") {
            menuData.SubTitle = " ";
        }

        menu = API.createMenu(menuData.Title, menuData.SubTitle, menuData.PosX, menuData.PosY, menuData.Anchor, menuData.EnableBanner);

        if (menuData.BannerSprite !== undefined) {
            API.setMenuBannerSprite(menu, menuData.BannerSprite.Dict, menuData.BannerSprite.Name);
        } else if (menuData.BannerTexture !== undefined) {
            API.setMenuBannerTexture(menu, menuData.BannerTexture);
        } else if (menuData.BannerColor !== undefined) {
            API.setMenuBannerRectangle(menu, menuData.BannerColor.alpha, menuData.BannerColor.red, menuData.BannerColor.green, menuData.BannerColor.blue);
        }

        for (let i = 0; i < menuData.Items.length; i++) {
            let item = menuData.Items[i];
            let menuItem;

            if (item.Id === undefined) {
                item.Id = "";
            }

            if (item.Text === undefined) {
                item.Text = "";
            }

            if (item.Description === undefined) {
                item.Description = "";
            }

            if (item.Type === 0 || item.Type === 2) {

                if (item.Type === 2) {
                    menuItem = API.createColoredItem(item.Text, item.Description, item.BackgroundColor, item.HighlightColor);
                } else {
                    menuItem = API.createMenuItem(item.Text, item.Description);
                }

                if (item.RightBadge !== undefined) {
                    menuItem.SetRightBadge(eval(item.RightBadge));
                }

                if (item.RightLabel !== undefined) {
                    menuItem.SetRightLabel(item.RightLabel);
                }
            }
            else if (item.Type === 1) {
                menuItem = API.createCheckboxItem(item.Text, item.Description, item.Checked);
            }
            else if (item.Type === 3) {
                let listItems = new List(String);

                for (let j = 0; j < item.Items.length; j++) {
                    listItems.Add(item.Items[j]);
                }

                menuItem = API.createListItem(item.Text, item.Description, listItems, item.SelectedItem);
            }

            if (item.LeftBadge !== undefined) {
                menuItem.SetLeftBadge(eval(item.LeftBadge));
            }

            menu.AddItem(menuItem);
            menuItems[i] = menuItem;
        }

        if (menuData.NoExit) {
            menu.ResetKey(menuControl.Back);
        }

        menu.OnIndexChange.connect((sender, item) => {
            if (menuData.OnIndexChange !== undefined) {
                eval(menuData.OnIndexChange);
            }
        });

        menu.OnCheckboxChange.connect((sender, item, checked) => {
            menuData.Items[getIndexOfMenuItem(item)].Checked = checked;

            if (menuData.OnCheckboxChange !== undefined) {
                eval(menuData.OnCheckboxChange);
            }
        });

        menu.OnListChange.connect((sender, item, index) => {
            menuData.Items[getIndexOfMenuItem(item)].SelectedItem = index;

            if (menuData.OnListChange !== undefined) {
                eval(menuData.OnListChange);
            }
        });

        menu.OnItemSelect.connect((sender, item, index) => {
            let menuItem = menuData.Items[index];

            if (menuItem.InputMaxLength > 0) {
                if (menuItem.RightLabel === undefined) {
                    menuItem.RightLabel = "";
                }

                let input = API.getUserInput(menuItem.RightLabel, menuItem.InputMaxLength);
                let valid = true;

                if (menuItem.InputType === 1) {
                    input = input.trim();

                    if (input.length !== 0) {
                        input = parseInt(input);

                        if (isNaN(input)) {
                            valid = false;
                        } else {
                            input = input.toString();
                        }
                    }
                } else if (menuItem.InputType === 2) {
                    input = input.trim();

                    if (input.length !== 0) {
                        input = parseInt(input);

                        if (isNaN(input) || input < 0) {
                            valid = false;
                        } else {
                            input = input.toString();
                        }
                    }
                } else if (menuItem.InputType === 3) {
                    input = input.trim();

                    if (input.length !== 0) {
                        input = parseFloat(input);

                        if (isNaN(input)) {
                            valid = false;
                        } else {
                            input = input.toString();
                        }
                    }
                }

                if (valid) {
                    menuItem.RightLabel = input;
                    item.SetRightLabel(input);
                }
            }

            if (menuData.OnItemSelect !== undefined) {
                eval(menuData.OnItemSelect);
            }

            if (menuItem.ExecuteCallback) {
                let resultData = new Array();

                for (let i = 0; i < menuData.Items.length; i++) {
                    menuItem = menuData.Items[i];

                    if (menuItem.Type === 1) {
                        resultData[menuItem.Id] = menuItem.Checked;
                    } else if (menuItem.Type === 3) {
                        resultData[menuItem.Id] = new Array();
                        resultData[menuItem.Id]["Index"] = menuItem.SelectedItem;
                        resultData[menuItem.Id]["Value"] = menuItem.Items[menuItem.SelectedItem];
                    } else if (menuItem.InputMaxLength > 0 && menuItem.RightLabel != null && menuItem.RightLabel.length > 0) {
                        resultData[menuItem.Id] = menuItem.RightLabel;
                    }
                }

                API.triggerServerEvent("MenuManager_ExecuteCallback", menuData.Id, menuItem.Id, index, API.toJson(resultData));
            }
        });

        menu.OnMenuClose.connect(() => {
            if (menuData.BackCloseMenu === false) {
                menu.Visible = true;
            }

            if (menuData.OnMenuClose !== undefined) {
                eval(menuData.OnMenuClose);
            }

            API.triggerServerEvent("MenuManager_ClosedMenu");
        });

        if (menuData.OnMenuOpen !== undefined) {
            eval(menuData.OnMenuOpen);
        }

        menu.Visible = true;
    } else if (name === "MenuManager_CloseMenu") {
        menu.Visible = false;
    }
});

function getIndexOfMenuItem(menuItem) {
    for (let i = 0; i < menuItems.length; i++) {
        if (menuItems[i] === menuItem) {
            return i;
        }
    }

    return -1;
}
