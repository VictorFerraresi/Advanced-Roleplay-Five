using System;
using System.IO;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;

/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//////////////////// ANIMATOR - Animation Viewer [v1.0] /////////////////////
//////////////////////////// Author: Kamil Hazes ////////////////////////////
/////////////////////////////////////////////////////////////////////////////
/////////////// Simple animation preview script that lets you ///////////////
//////////// view all the animations included in animation list /////////////
/////////////////////////// on GTA:Network's Wiki! //////////////////////////
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////

public class Animator : Script
{
    public Animator()
    {
        API.onClientEventTrigger += OnClientEvent;
    }

    public void OnClientEvent(Client player, string eventName, params object[] args)
    {
        if (eventName == "PlayAnimation")
        {
            string[] animInfo = args[0].ToString().Split(' ');
            string anim_group = animInfo[0];
            string anim_name = animInfo[1];
            player.setData("PLAYED_ANIMATION_GROUP", anim_group);
            player.setData("PLAYED_ANIMATION_NAME", anim_name);
            player.stopAnimation();
            player.playAnimation(anim_group, anim_name, 9);
        }
    }

    [Command("animator", "====================[~b~ANIM~w~ATOR]====================\n"+
        "Launch: ~g~/animator start \n" +
        "~w~Help: ~g~/animator help \n" +
        "~w~Stop animation: ~g~/animator stop \n" +
        "~w~Save animation: ~g~/animator save [savename] \n" +
        "~w~Skip to animation: ~g~/animator skip [animation id]")]
    public void StartAnimator(Client player, string action = null, string action2 = "Anim")
    {
        if (!player.hasData("ANIMATOR_OPEN")) player.setData("ANIMATOR_OPEN", false);
        bool AnimatorOpen = player.getData("ANIMATOR_OPEN");

        if (action == null)
        {
            if (!AnimatorOpen)
            {
                player.setData("ANIMATOR_OPEN", true);
                player.triggerEvent("StartClientAnimator", animator.animations.AllAnimations);
                player.sendChatMessage("~b~[ANIMATOR]: ~w~Animator is now ~g~ON~w~. Type ~g~/animator help ~w~for more options.");
            }
            else
            {
                player.resetData("ANIMATOR_OPEN");
                player.triggerEvent("StopClientAnimator");
                player.stopAnimation();
                player.sendChatMessage("~b~[ANIMATOR]: ~w~Animator is now ~r~OFF~w~.");
            }
        }

        if(action != null && AnimatorOpen)
        {
            if (AnimatorOpen)
            {
                if (action == "save") SaveAnimatorData(player, action2);

                if (action == "skip") SkipAnimatorData(player, action2);

                if (action == "help")
                {
                    player.sendChatMessage("=================================[~b~ANIM~w~ATOR]================================");
                    player.sendChatMessage("Use ~y~LEFT ~w~and ~y~RIGHT ~w~arrow keys to cycle through animations.");
                    player.sendChatMessage("Use ~y~UP ~w~and ~y~DOWN ~w~arrow keys to cycle animations by one hundred instances.");
                    player.sendChatMessage("You may also skip to a specific animation ID, just use ~y~/animator skip [number]~w~.");
                    player.sendChatMessage("If you wish to save your animations into .txt file, use ~y~/animator save [savename]~w~.");
                    player.sendChatMessage("~w~Please remember that some animations are not meant to be used by peds, or");
                    player.sendChatMessage("are meant to be used in specific circumstances, therefore ~r~may not work~w~!");
                }

                if(action == "stop")
                {
                    player.sendChatMessage("~b~[ANIMATOR]: ~w~Animation have been stopped.");
                    player.stopAnimation();
                }
            } else
            {
                player.sendChatMessage("~b~[ANIMATOR]: ~r~You have to launch the animator first! ~y~/animator.");
            }
        }
    }

    public void SaveAnimatorData(Client player, string name)
    {
        string anim_group = player.getData("PLAYED_ANIMATION_GROUP");
        string anim_name = player.getData("PLAYED_ANIMATION_NAME");
        File.AppendAllText("Saved_Animations.txt", string.Format("{0}:          {1} {2}", name, anim_group, anim_name) + Environment.NewLine);
        player.sendChatMessage(string.Format("~b~[ANIMATOR]: ~w~Animation saved! Name: ~g~{0} ~w~Anim: ~y~{1} ~b~{2}", name, anim_group, anim_name));
    }

    public void SkipAnimatorData(Client player, string animationID)
    {
        int ID;
        if (Int32.TryParse(animationID, out ID))
        {
            int animations_amount = animator.animations.AllAnimations.Count - 1;
            if (ID > animations_amount || ID < 0)
            {
                player.sendChatMessage("~b~[ANIMATOR]: ~w~ID has to be between 0 and "+ animations_amount+"!");
                return;
            }
            player.triggerEvent("SkipAnimatorData", ID);
        } else {
            player.sendChatMessage("~b~[ANIMATOR]: ~r~Wrong number format!");
        }
    }
}