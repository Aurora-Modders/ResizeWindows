using System;
using System.Linq;
using System.Windows.Forms;

using HarmonyLib;

namespace ResizeWindows
{
    public class ResizeWindows : AuroraPatch.Patch
    {
        public override string Description => "Allow for resizing windows.";

        protected override void Loaded(Harmony harmony)
        {
            LogInfo("Loading ResizeWindows...");
            HarmonyMethod formConstructorPostfixMethod = new HarmonyMethod(GetType().GetMethod("FormConstructorPostfix", AccessTools.all));
            foreach (var form in AuroraAssembly.GetTypes().Where(t => typeof(Form).IsAssignableFrom(t)))
            {
                try
                {
                    foreach (var ctor in form.GetConstructors())
                    {
                        harmony.Patch(ctor, postfix: formConstructorPostfixMethod);
                    }
                }
                catch (Exception e)
                {
                    LogError($"ResizeWindows failed to patch Form constructor {form.Name}, exception: {e}");
                }
            }
        }

        /// <summary>
        /// Harmony method to postfix the Form constructor in order to add our custom HandleCreated callback.
        /// </summary>
        /// <param name="__instance"></param>
        private static void FormConstructorPostfix(Form __instance)
        {
            __instance.HandleCreated += CustomHandleCreated;
        }

        /// <summary>
        /// Our custom handle created callback will set our sizable property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CustomHandleCreated(Object sender, EventArgs e)
        {
            Form form = sender as Form;
            form.FormBorderStyle = FormBorderStyle.Sizable;
            form.AutoScroll = true;
        }
    }
}
