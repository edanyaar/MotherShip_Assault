Hello.
Allow me to thank you for purchasing this asset. Hope you'll find it useful.

Asset contains all files needed for creating dynamic forcefield effect on any mesh.

Main files needed for creating effect:
1. ForceField.cs - script that creates forcefield effect on an object it is attached to. Requires MeshRenderer and any Collider compoent.

2. ForceFieldAdditive or ForceFieldAlphaBlend shader which will display the effect.

To create force field:
1. Attach ForceField.cs to object that will be the forcefield
2. If needed, replace Additive shader set by default with Alpha Blend version
3. Configure effect parameters to your liking


Parameters description:
1.Impact Sound:
	AudioClip that will be played after impact.

2.Impact On Collision:
	Detirmines if collisions will trigger impact effect.

3.Scale:
	Here you set scale of forcefield object.

4.Field Color:
	Color of the field.



//Field splash parameters//

5.Field Splash Lifetime:
	Time from field splash appear to splash fully disappear.

6.Field Splash Radius:
	Initial radius of splash of the field around impact point.

7.Field Splash Multiplier:
	How much splash radius will grow by the end of it's lifetime.

8.Field Splash Pow:
	Math Pow coef for splash spot falloff.

9.Field Splash Decay Start After LifeTime Percent:
	After that percent of lifetime field splash will start to fade out.



//Impact splash parameters//

10.Impact Splash Lifetime:
	Time from impact splash appear to splash fully disappear.

11.ImpactSplashRadius:
	Initial radius of the impact splash at impact point.

12.ImpactSplashVisibility:
	Visibility of the impact splash.

13.ImpactSplashPow:
	Math Pow coef for impact splash spot falloff.

14.ImpactSplashGrowthMultiplier:
	How much impact splash radius will grow by the end of it's lifetime.


//Field passive visibility parameters//

15.PassiveFieldVisibility:
	Visibility of the field when there are no impacts. (added to rim visibility. set to 0.5 to match field and rim visibility)

16.FieldRimVisibility:
	Visibility of the field rim around object.


17.FieldRimColorVisibility:
	Visibility of additional color added to field effect.

18.FieldDischargeVisibility:
	Visibility of the field splash.



//Field solid color highlight visibility parameters//

FieldHighlightVisibility:
	Visibility of the solid color highlight of the field splash.

FieldHighlightPow:
	Math Pow coef for the solid color highlight falloff.



//Field mesh outline parameters//
FieldOutlineThickness:
	Thickness of the mesh outline. 



//Field 3d Noise parameters (selfexplanatory)//
Octaves

Frequency

Amplitude

Lacunarity

Persistence

Offset

RidgeOffset

AnimSpeed



//Math Pow coefs of the field rim faloff//
PowInner

PowOuter

PowInner2

PowOuter2


----------//License//-----------
This software can be used, sold, redistributed and modified in any way, provided that it was legally purchased.


----------//3D Noise License//--------------------

//	Code repository for GPU noise development blog
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//
//	Brian Sharpe
//	brisharpe CIRCLE_A yahoo DOT com
//	http://briansharpe.wordpress.com
//	https://github.com/BrianSharpe
//

//===============================================================================
//  Scape Software License
//===============================================================================
//
//Copyright (c) 2007-2012, Giliam de Carpentier
//All rights reserved.
//
//Redistribution and use in source and binary forms, with or without
//modification, are permitted provided that the following conditions are met: 
//
//1. Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer. 
//2. Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNERS OR CONTRIBUTORS BE LIABLE 
//FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
//OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.;







