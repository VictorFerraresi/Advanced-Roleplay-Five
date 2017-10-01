var resolutionX = API.getScreenResolutionMaintainRatio().Width;
var resolutionY = API.getScreenResolutionMaintainRatio().Height;
var CurrentAnimation = 0;
var Animations = {};
var AnimatorLaunched = false;
var player = API.getLocalPlayer();

API.onServerEventTrigger.connect(function (evName, args) {

    if (evName == "StartClientAnimator") {
        if (!AnimatorLaunched) {
            Animations = args[0];
            AnimatorLaunched = true;
            PlayAnimation(Animations[CurrentAnimation]);
        }
    }

    if (evName == "StopClientAnimator") {
        Animations = null;
        AnimatorLaunched = false;
    }

    if (evName == "SkipAnimatorData") {
        if (args[0] >= Animations.Count || args[0] < 0) return;
        CurrentAnimation = args[0];
        var AnimationInfo = Animations[CurrentAnimation];
        PlayAnimation(AnimationInfo);
    }
});

API.onUpdate.connect(function () {
    if (AnimatorLaunched) {
        var AnimationInfo = Animations[CurrentAnimation];
        var Animation = AnimationInfo.split(' ');
        API.drawText("Animation: ", resolutionX - 250, resolutionY - 200, 0.7, 255, 255, 255, 255, 6, 2, false, true, 0);
        API.drawText("~g~[ " + CurrentAnimation + " ] ~y~" + Animation[0].toString() + " ~b~" + Animation[1].toString(), resolutionX - 250, resolutionY - 165, 0.6, 255, 255, 255, 255, 6, 2, false, true, 0);
    }
});

API.onKeyDown.connect(function (sender, e) {
    if (AnimatorLaunched) {
        var AnimationInfo = Animations[CurrentAnimation];
        if (e.KeyCode === Keys.Right || e.KeyCode === Keys.Left || e.KeyCode === Keys.Up || e.KeyCode === Keys.Down) {
            if (e.KeyCode === Keys.Right) {
                if (CurrentAnimation < Animations.Count-1) {
                    CurrentAnimation++;
                }
            }
            if (e.KeyCode === Keys.Left) {
                if(CurrentAnimation > 0) {
                    CurrentAnimation--;
                }
            }
            if (e.KeyCode === Keys.Up) {
                if (CurrentAnimation + 100 < Animations.Count-1) {
                    CurrentAnimation += 100;
                }
            }
            if (e.KeyCode === Keys.Down) {
                if (CurrentAnimation - 100 > 0) {
                    CurrentAnimation -= 100;
                }
            }
        }
    }
});

API.onKeyUp.connect(function (sender, e) {
    if (AnimatorLaunched) {
        if (e.KeyCode === Keys.Right || e.KeyCode === Keys.Left || e.KeyCode === Keys.Up || e.KeyCode === Keys.Down) {
            var AnimationInfo = Animations[CurrentAnimation];
            PlayAnimation(AnimationInfo);
        }
    }
});

function PlayAnimation(AnimInfo) {
    API.triggerServerEvent("PlayAnimation", AnimInfo);
}