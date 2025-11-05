using MonoDetour.HookGen;

[assembly: MonoDetourTargets(typeof(InputHandler), GenerateControlFlowVariants = true)]
[assembly: MonoDetourTargets(typeof(HeroController))]
