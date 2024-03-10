using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using System.Reflection;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TigerLearning.Learning;

public class 泰拉瑞亚On {
    public class 重写伤害计算 : ModSystem {
        public const string 参考 = "On的功能与应用 https://fs49.org/2021/10/28/on%e7%9a%84%e5%8a%9f%e8%83%bd%e4%b8%8e%e5%ba%94%e7%94%a8/";
        //其实来说这种只改一处的东西最好放在IL中(但我暂时不会)
        public static bool ChangePlayerGetDamage = true;    //一般为写在配置里
        public static bool ChangeNPCGetDamage = true;
        public override void Load() {
            On_Player.HurtModifiers.GetDamage += HookPlayerGetDamage;
            On_NPC.HitModifiers.GetDamage += HookNPCGetDamage;
        }
        public override void Unload() {
            On_Player.HurtModifiers.GetDamage -= HookPlayerGetDamage;
            On_NPC.HitModifiers.GetDamage -= HookNPCGetDamage;
        }
        public static float HookPlayerGetDamage(On_Player.HurtModifiers.orig_GetDamage orig, ref Player.HurtModifiers self, float baseDamage, float defense, float defenseEffectiveness) {
            if(ChangePlayerGetDamage) {
                float damage = self.SourceDamage.ApplyTo(baseDamage) * self.IncomingDamageMultiplier.Value;
                float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                defense = Math.Max(defense - armorPenetration, 0);

                float damageReduction = defense * defenseEffectiveness;
                damage = Math.Max(damage * 100 / (damageReduction + 100), 1);   //重点在这里

                return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
            }
            else {
                //原版的源码, 相当于orig(self, baseDamage, defense, defenseEffectiveness);
                float damage = self.SourceDamage.ApplyTo(baseDamage) * self.IncomingDamageMultiplier.Value;
                float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                defense = Math.Max(defense - armorPenetration, 0);

                float damageReduction = defense * defenseEffectiveness;
                damage = Math.Max(damage - damageReduction, 1);

                return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
            }
        }
        public static int HookNPCGetDamage(On_NPC.HitModifiers.orig_GetDamage orig, ref NPC.HitModifiers self, float baseDamage, bool crit, bool damageVariation, float luck) {
            if(ChangeNPCGetDamage) {
                bool? _critOverride = (bool?)typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);
                if(self.SuperArmor) {
                    float dmg = 1;

                    if(_critOverride ?? crit)
                        dmg *= self.CritDamage.Additive * self.CritDamage.Multiplicative;

                    return Math.Clamp((int)dmg, 1, 10);
                }

                float damage = self.SourceDamage.ApplyTo(baseDamage);
                damage += self.FlatBonusDamage.Value + self.ScalingBonusDamage.Value * damage;
                damage *= self.TargetDamageMultiplier.Value;

                int variationPercent = Utils.Clamp((int)Math.Round(Main.DefaultDamageVariationPercent * self.DamageVariationScale.Value), 0, 100);
                if(damageVariation && variationPercent > 0)
                    damage = Main.DamageVar(damage, variationPercent, luck);

                float defense = self.Defense.ApplyTo(0);
                float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                defense = Math.Max(defense - armorPenetration, 0);

                float damageReduction = defense * self.DefenseEffectiveness.Value;
                damage = Math.Max(damage * 100 / (damageReduction + 100), 1);     //重点在这里

                if(_critOverride ?? crit)
                    damage = self.CritDamage.ApplyTo(damage);

                return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
            }
            else {
                //原版的源码, 相当于orig(self, baseDamage, crit, damageVariation, luck);
                bool? _critOverride = (bool?)typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.NonPublic).GetValue(self);
                if(self.SuperArmor) {
                    float dmg = 1;

                    if(_critOverride ?? crit)
                        dmg *= self.CritDamage.Additive * self.CritDamage.Multiplicative;

                    return Math.Clamp((int)dmg, 1, 10);
                }

                float damage = self.SourceDamage.ApplyTo(baseDamage);
                damage += self.FlatBonusDamage.Value + self.ScalingBonusDamage.Value * damage;
                damage *= self.TargetDamageMultiplier.Value;

                int variationPercent = Utils.Clamp((int)Math.Round(Main.DefaultDamageVariationPercent * self.DamageVariationScale.Value), 0, 100);
                if(damageVariation && variationPercent > 0)
                    damage = Main.DamageVar(damage, variationPercent, luck);

                float defense = self.Defense.ApplyTo(0);
                float armorPenetration = defense * Math.Clamp(self.ScalingArmorPenetration.Value, 0, 1) + self.ArmorPenetration.Value;
                defense = Math.Max(defense - armorPenetration, 0);

                float damageReduction = defense * self.DefenseEffectiveness.Value;
                damage = Math.Max(damage - damageReduction, 1);

                if(_critOverride ?? crit)
                    damage = self.CritDamage.ApplyTo(damage);

                return Math.Max((int)self.FinalDamage.ApplyTo(damage), 1);
            }
        }
    }
    public class 动态icon : ModSystem {
        public const string 参考 = "使用反射实现动态Icon https://fs49.org/2022/01/18/%e4%bd%bf%e7%94%a8%e5%8f%8d%e5%b0%84%e5%ae%9e%e7%8e%b0%e5%8a%a8%e6%80%81icon/";
        //其实来说也可以直接写在UpdateUI里面(虽说Hook到DrawMenu确实更加精确)
        //所此处更应该说是对C#反射和泰拉瑞亚UI的应用
        int timer, iconFrame;
        Asset<Texture2D>[] icons;
        Asset<Texture2D> Icon => icons[iconFrame];
        public override void Load() {
            On_Main.DrawMenu += HookDrawMenu;
            icons = new Asset<Texture2D>[7];    //7: 总共多少张图
            foreach(int i in Range(icons.Length)) {
                icons[i] = ModContent.Request<Texture2D>($"ExampleMod/Images/Icons/Icon{i}");
            }
        }
        public override void Unload() {
            On_Main.DrawMenu -= HookDrawMenu;
        }
        void HookDrawMenu(On_Main.orig_DrawMenu orig, Main self, GameTime time) {
            //其实来说如此频繁地反射应该把FieldInfo存起来
            BindingFlags puf = BindingFlags.Public | BindingFlags.Instance, prf = BindingFlags.NonPublic | BindingFlags.Instance;
            Main.MenuUI.GetField(out List<UIState> _history, "_history", puf);
            UIState his = _history.Find(s => s.GetType().FullName == "Terraria.ModLoader.UI.UIMods");
            if(his == null) {
                goto EndSetIcon;
            }
            //获得UIMods(继承自UIState)的元素elements
            his.GetField(out List<UIElement> elements, "Elements", prf);
            //elements[0]则是uiElement(参见Terraria.ModLoader.UI.UIMods.OnInitialize())
            elements[0].GetField(out List<UIElement> uiElements, "Elements", prf);
            //获得uiElement的元素uiElements, uiElement.Elements[0]则是uiPanel
            uiElements[0].GetField(out List<UIElement> panelElements, "Elements", prf);
            //uiPanel.Elements[0]为modList, 好在modList是一个UIList, 我们可以直接使用它
            var modItem = uiElements.Find(e => e.GetField("_mod", prf).ToString() == Name);
            if(modItem == null) {
                goto EndSetIcon;
            }
            //找到我们的模组, 那么modItem.Elements[0]若为UIImage则必为图标(参见Terraria.ModLoader.UI.UIModItem.OnInitialize())
            //其实保险起见也应该写for循环的(
            modItem.GetField(out List<UIElement> itemElements, "Elements", prf);
            if(itemElements[0] is not UIImage uiImage) {
                goto EndSetIcon;
            }
            uiImage.SetImage(Icon);
        //待测试: 是否需要调用SetField
        EndSetIcon:
            #region 计时
            timer += 1;      //timer += time.ElapsedGameTime.TotalSeconds;
            if(timer >= 6) {
                timer -= 6;
                iconFrame += 1;
                if(iconFrame >= icons.Length) {
                    iconFrame %= icons.Length;  //iconFrame -= icons.Length
                }
            }
            #endregion
            orig(self, time);
        }
    }
    public class 给任意方法上钩子 {
        public const string 参考 = "通过 HookEndpointManager 动态挂钩子 https://fs49.org/2022/11/20/%e9%80%9a%e8%bf%87-hookendpointmanager-%e5%8a%a8%e6%80%81%e6%8c%82%e9%92%a9%e5%ad%90/";
        public static Type 主角 = typeof(MonoModHooks);
        /// <summary>
        /// 代表原方法, 若为静态方法则没有<paramref name="self"/>
        /// </summary>
        /// <param name="self">
        /// <br/>此类的实例
        /// <br/>当方法为静态方法时没有这个参数
        /// <br/>当此类为值类型时此参数需为ref引用
        /// </param>
        /// <param name="parameters">此方法的参数, 实际使用时需用对应参数替代</param>
        /// <returns>若此方法没有返回值则返回类型为void</returns>
        delegate object MethodDelegate(object self, params object[] parameters);
        static object MethodHook(MethodDelegate orig, object self, params object[] parameters) {
            //要改什么在这里写(On)
            return orig.Invoke(self, parameters);
        }
        static void Manipulate(ILContext il) {
            //IL代码写在这儿
        }
        public static void ShowHooks() {
            MethodBase method = default;    //使用反射获取
            MonoModHooks.Add(method, MethodHook);   //ON, 会在Mod卸载时自动卸载
            MonoModHooks.Modify(method, Manipulate);  //IL, 但由于我不知道所以就放这儿
        }
    }
    public class 给任意方法上钩子而且不自动卸载的那种_可脱离TML使用 {
        public delegate object MethodDelegate(object self, params object[] parameters);
        public static object MethodHook(MethodDelegate orig, object self, params object[] parameters) {
            //这里写On
            return orig(self, parameters);
        }
        public static void Manipulate(ILContext il) {
            //这里写IL
        }
        public static Hook hook;
        public static ILHook ilHook;
        public static void ShowHook() {
            #region params
            MethodBase method = default;
            MethodInfo anotherMethod = default;
            bool applyByDefault = default;
            DetourConfig detourConfig = default;
            object targetObject = default;
            #endregion
            #region On钩子
            hook = new(method, MethodHook);     //挂On钩子
            hook = new(method, MethodHook, true);     //在初始化后立即挂上去(其他的版本基本都有这个多的applyByDefault参数, 默认为true)
            hook = new(method, MethodHook, detourConfig);   //配置挂钩子的顺序, 优先级等, 默认空
            hook = new(method, MethodHook, detourConfig, applyByDefault);   //以上两种一起(基本每个版本都有这多的三种变体, 后不一一列举)
            hook = new(method, anotherMethod);  //直接用anotherMethod替换method(TBT)
            hook = new(method, anotherMethod, targetObject);    //修改特定对象的方法(必须直接替换, 不能用MethodHook)(同样有上面的三种变体)
            Show(hook.Config);      //获得配置, 若无则是空
            hook.Apply();           //挂上此钩子
            hook.Undo();            //卸载此钩子
            Show(hook.IsApplied);   //是否挂上了钩子
            #endregion
            #region IL钩子
            ilHook = new(method, Manipulate);
            ilHook = new(method, Manipulate, applyByDefault);   //是否立即挂上去(默认false)
            ilHook = new(method, Manipulate, detourConfig);     //配置挂钩子的顺序, 优先级等, 默认空
            ilHook = new(method, Manipulate, detourConfig, applyByDefault);//以上两种一起
            Show(ilHook.Config);    //获得配置, 若无则是空
            ilHook.Apply();         //挂上此钩子
            ilHook.Undo();          //卸载此钩子
            Show(ilHook.IsApplied); //是否挂上了钩子
            #endregion
        }
    }
}