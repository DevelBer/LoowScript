using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Vayne_1v.Util
{
    class Vayne
    {
        static Menu menu;

        static float hisstaa, peckmove, tdelay, Onidelay;

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            menu = MainMenu.AddMenu("Vayne", "Vayne");
            menu.Add("Combo", new KeyBind("Combo", false, KeyBind.BindTypes.HoldActive, ' '));
            Game.OnUpdate += Game_OnTick;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe && args.Buff.Name == "Vayne 1v") ;
        }

        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
			{
                tdelay = sender.AttackCastDelay;
                Onidelay = sender.AttackDelay;
                hisstaa = Game.Time;
			}
            
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (menu["Combo"].Cast<KeyBind>().CurrentValue)
			{
				Orbwalker.DisableMovement = true;
				Orbwalker.DisableAttacking = true;
				Orb();
			}
			else
			{
				Orbwalker.DisableMovement = false;
				Orbwalker.DisableAttacking = false;
			}
		}
		
		static void Orb()
		{
			if (Player.CanUseSpell(SpellSlot.Q) == SpellState.Ready && Game.Time > hisstaa + tdelay + 0.025f && Game.Time < hisstaa + (Onidelay * 0.75f))
			{
				Player.CastSpell(SpellSlot.Q, Game.CursorPos);
				return;
			}
			var target = GetAATarget(Player.Instance.AttackRange + Player.Instance.BoundingRadius);
			if (target == null)
			{
				if (Game.Time > hisstaa + tdelay + 0.025f && Game.Time > peckmove + 0.150f)
				{
					Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
					peckmove = Game.Time;
				}
				return;
			}
			if (Game.Time > hisstaa + Onidelay)
			{
				Player.IssueOrder(GameObjectOrder.AttackUnit, target);
				return;
			}
			if (Game.Time > hisstaa + tdelay + 0.025f && Game.Time > peckmove + 0.150f)
			{
				Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
				peckmove = Game.Time;
			}
		}
		
		static AttackableUnit GetAATarget(float range)
		{
			AttackableUnit t = null;
			float num = 10000;
			foreach (var enemy in EntityManager.Heroes.Enemies)
			{
				float hp = enemy.Health;
				if (enemy.IsValidTarget(range + enemy.BoundingRadius) && hp < num)
				{
					num = hp;
					t = enemy;
				}
			}
			return t;
		}
            
        }
    }
