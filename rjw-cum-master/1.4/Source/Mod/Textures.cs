using Verse;
using UnityEngine;
//using Multiplayer.API;

namespace rjwcum
{
	[StaticConstructorOnStartup]
	public static class CumTextures
	{
		//UI:
		public static readonly Texture2D CumIcon_little = ContentFinder<Texture2D>.Get("CumIcon_little", true);
		public static readonly Texture2D CumIcon_some = ContentFinder<Texture2D>.Get("CumIcon_some", true);
		public static readonly Texture2D CumIcon_dripping = ContentFinder<Texture2D>.Get("CumIcon_dripping", true);
		public static readonly Texture2D CumIcon_drenched = ContentFinder<Texture2D>.Get("CumIcon_drenched", true);

		//on pawn:
		public static readonly Material cumSplatch1 = MaterialPool.MatFrom("splatch_1", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch2 = MaterialPool.MatFrom("splatch_2", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch3 = MaterialPool.MatFrom("splatch_3", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch4 = MaterialPool.MatFrom("splatch_4", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch5 = MaterialPool.MatFrom("splatch_5", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch6 = MaterialPool.MatFrom("splatch_6", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch7 = MaterialPool.MatFrom("splatch_7", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch8 = MaterialPool.MatFrom("splatch_8", ShaderDatabase.Cutout);
		public static readonly Material cumSplatch9 = MaterialPool.MatFrom("splatch_9", ShaderDatabase.Cutout);


		//[SyncMethod]
		public static Material pickRandomSplatch()
		{
			//Rand.PopState();
			//Rand.PushState(RJW_Multiplayer.PredictableSeed());
			int rand = Rand.Range(0, 8);
			switch (rand)
			{
				case 0:
					return cumSplatch1;
				case 1:
					return cumSplatch2;
				case 2:
					return cumSplatch3;
				case 3:
					return cumSplatch4;
				case 4:
					return cumSplatch5;
				case 5:
					return cumSplatch6;
				case 6:
					return cumSplatch7;
				case 7:
					return cumSplatch8;
				case 8:
					return cumSplatch9;
			}
			return null;
		}
	}
}
