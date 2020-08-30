// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "ForceField/ForceFieldShaderAdd" {
	Properties {
		
		_FieldColor("Field Color",Color)=(0,0.5,1,1)
		_FieldSplashRadius("Field Splash Radius", float)=1
		_FieldSplashGrowthMultiplier("Field Splash Growth Multiplier", float)=4

		_FieldSplashPow("Field Splash Pow",float)=3
		_FieldSplashDecayStartAfterLifeTimePercent("Field Splash Decay Start After LifeTime %",float)=20


		_ImpactSplashRadius("Impact Splash Radius", float)=0.5
		_ImpactSplashVisibility("Impact Splash Visibility",Range(0,1.5))=1
		_ImpactSplashPow("Impact Splash Pow",float)=2.6
		_ImpactSplashGrowthMultiplier("Impact Splash Growth Multiplier", float)=3

		_PassiveFieldVisibility("Passive Field Visibility",Range(0,0.5))=0.05
		_FieldRimVisibility("Field Rim Visibility",Range(0,1))=0.15
		_FieldRimColorVisibility("Field Color Rim Visibility",Range(0,1))=1

		_FieldDischargeVisibility("Field Discharge Visibility",Range(0,1))=0.5

		_FieldHighlightVisibility("Field Highlight Visibility",Range(0,1))=0.6
		_FieldHighlightPow("Field Highlight Pow",float)=1.8

		_FieldOutlineThickness("Field Outline Thickness",float)=0.0



		_Octaves ("Octaves", Float) = 3.0
		_Frequency ("Frequency", Float) = 2
		_Amplitude ("Amplitude", Float) = 0.9
		_Lacunarity ("Lacunarity", Float) = 2.5
		_Persistence ("Persistence", Float) = 0.5
		_Offset ("Offset", Vector) = (0.0, 0.0, 0.0, 0.0)
		_RidgeOffset ("Ridge Offset", Float) = 0.15
		_AnimSpeed("Anim Speed", float)=10

		_PowInner("Pow Inner",float)=1.5
		_PowOuter("Pow Outer",float)=5

		_PowInner2("Pow Inner 2",float)=2
		_PowOuter2("Pow Outer 2",float)=5

	}






	CGINCLUDE
		//
		//	FAST32_hash
		//	A very fast hashing function.  Requires 32bit support.
		//	http://briansharpe.wordpress.com/2011/11/15/a-fast-and-simple-32bit-floating-point-hash-function/
		//
		//	The hash formula takes the form....
		//	hash = mod( coord.x * coord.x * coord.y * coord.y, SOMELARGEFLOAT ) / SOMELARGEFLOAT
		//	We truncate and offset the domain to the most interesting part of the noise.
		//	SOMELARGEFLOAT should be in the range of 400.0->1000.0 and needs to be hand picked.  Only some give good results.
		//	3D Noise is achieved by offsetting the SOMELARGEFLOAT value by the Z coordinate
		//
		void FAST32_hash_3D( 	float3 gridcell,
								float3 v1_mask,		//	user definable v1 and v2.  ( 0's and 1's )
								float3 v2_mask,
								out float4 hash_0,
								out float4 hash_1,
								out float4 hash_2	)		//	generates 3 random numbers for each of the 4 3D cell corners.  cell corners:  v0=0,0,0  v3=1,1,1  the other two are user definable
		{
			//    gridcell is assumed to be an integer coordinate
		
			//	TODO: 	these constants need tweaked to find the best possible noise.
			//			probably requires some kind of brute force computational searching or something....
			const float2 OFFSET = float2( 50.0, 161.0 );
			const float DOMAIN = 69.0;
			const float3 SOMELARGEFLOATS = float3( 635.298681, 682.357502, 668.926525 );
			const float3 ZINC = float3( 48.500388, 65.294118, 63.934599 );
		
			//	truncate the domain
			gridcell.xyz = gridcell.xyz - floor(gridcell.xyz * ( 1.0 / DOMAIN )) * DOMAIN;
			float3 gridcell_inc1 = step( gridcell, float3( DOMAIN - 1.5, DOMAIN - 1.5, DOMAIN - 1.5) ) * ( gridcell + 1.0 );
		
			//	compute x*x*y*y for the 4 corners
			float4 P = float4( gridcell.xy, gridcell_inc1.xy ) + OFFSET.xyxy;
			P *= P;
			float4 V1xy_V2xy = lerp( P.xyxy, P.zwzw, float4( v1_mask.xy, v2_mask.xy ) );		//	apply mask for v1 and v2
			P = float4( P.x, V1xy_V2xy.xz, P.z ) * float4( P.y, V1xy_V2xy.yw, P.w );
		
			//	get the lowz and highz mods
			float3 lowz_mods = float3( 1.0 / ( SOMELARGEFLOATS.xyz + gridcell.zzz * ZINC.xyz ) );
			float3 highz_mods = float3( 1.0 / ( SOMELARGEFLOATS.xyz + gridcell_inc1.zzz * ZINC.xyz ) );
		
			//	apply mask for v1 and v2 mod values
		    v1_mask = ( v1_mask.z < 0.5 ) ? lowz_mods : highz_mods;
		    v2_mask = ( v2_mask.z < 0.5 ) ? lowz_mods : highz_mods;
		
			//	compute the final hash
			hash_0 = frac( P * float4( lowz_mods.x, v1_mask.x, v2_mask.x, highz_mods.x ) );
			hash_1 = frac( P * float4( lowz_mods.y, v1_mask.y, v2_mask.y, highz_mods.y ) );
			hash_2 = frac( P * float4( lowz_mods.z, v1_mask.z, v2_mask.z, highz_mods.z ) );
		}
		//
		//	Given an arbitrary 3D point this calculates the 4 vectors from the corners of the simplex pyramid to the point
		//	It also returns the integer grid index information for the corners
		//
		void Simplex3D_GetCornerVectors( 	float3 P,					//	input point
											out float3 Pi,			//	integer grid index for the origin
											out float3 Pi_1,			//	offsets for the 2nd and 3rd corners.  ( the 4th = Pi + 1.0 )
											out float3 Pi_2,
											out float4 v1234_x,		//	vectors from the 4 corners to the intput point
											out float4 v1234_y,
											out float4 v1234_z )
		{
			//
			//	Simplex math from Stefan Gustavson's and Ian McEwan's work at...
			//	http://github.com/ashima/webgl-noise
			//
		
			//	simplex math constants
			const float SKEWFACTOR = 1.0/3.0;
			const float UNSKEWFACTOR = 1.0/6.0;
			const float SIMPLEX_CORNER_POS = 0.5;
			const float SIMPLEX_PYRAMID_HEIGHT = 0.70710678118654752440084436210485;	// sqrt( 0.5 )	height of simplex pyramid.
		
			P *= SIMPLEX_PYRAMID_HEIGHT;		// scale space so we can have an approx feature size of 1.0  ( optional )
		
			//	Find the vectors to the corners of our simplex pyramid
			Pi = floor( P + dot( P, float3( SKEWFACTOR, SKEWFACTOR, SKEWFACTOR) ) );
			float3 x0 = P - Pi + dot(Pi, float3( UNSKEWFACTOR, UNSKEWFACTOR, UNSKEWFACTOR ) );
			float3 g = step(x0.yzx, x0.xyz);
			float3 l = 1.0 - g;
			Pi_1 = min( g.xyz, l.zxy );
			Pi_2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - Pi_1 + UNSKEWFACTOR;
			float3 x2 = x0 - Pi_2 + SKEWFACTOR;
			float3 x3 = x0 - SIMPLEX_CORNER_POS;
		
			//	pack them into a parallel-friendly arrangement
			v1234_x = float4( x0.x, x1.x, x2.x, x3.x );
			v1234_y = float4( x0.y, x1.y, x2.y, x3.y );
			v1234_z = float4( x0.z, x1.z, x2.z, x3.z );
		}
		//
		//	Calculate the weights for the 3D simplex surflet
		//
		float4 Simplex3D_GetSurfletWeights( 	float4 v1234_x,
											float4 v1234_y,
											float4 v1234_z )
		{
			//	perlins original implementation uses the surlet falloff formula of (0.6-x*x)^4.
			//	This is buggy as it can cause discontinuities along simplex faces.  (0.5-x*x)^3 solves this and gives an almost identical curve
		
			//	evaluate surflet. f(x)=(0.5-x*x)^3
			float4 surflet_weights = v1234_x * v1234_x + v1234_y * v1234_y + v1234_z * v1234_z;
			surflet_weights = max(0.5 - surflet_weights, 0.0);		//	0.5 here represents the closest distance (squared) of any simplex pyramid corner to any of its planes.  ie, SIMPLEX_PYRAMID_HEIGHT^2
			return surflet_weights*surflet_weights*surflet_weights;
		}
		//
		//	SimplexPerlin3D  ( simplex gradient noise )
		//	Perlin noise over a simplex (tetrahedron) grid
		//	Return value range of -1.0->1.0
		//	http://briansharpe.files.wordpress.com/2012/01/simplexperlinsample.jpg
		//
		//	Implementation originally based off Stefan Gustavson's and Ian McEwan's work at...
		//	http://github.com/ashima/webgl-noise
		//
		float SimplexPerlin3D(float3 P)
		{
			//	calculate the simplex vector and index math
			float3 Pi;
			float3 Pi_1;
			float3 Pi_2;
			float4 v1234_x;
			float4 v1234_y;
			float4 v1234_z;
			Simplex3D_GetCornerVectors( P, Pi, Pi_1, Pi_2, v1234_x, v1234_y, v1234_z );
		
			//	generate the random vectors
			//	( various hashing methods listed in order of speed )
			float4 hash_0;
			float4 hash_1;
			float4 hash_2;
			FAST32_hash_3D( Pi, Pi_1, Pi_2, hash_0, hash_1, hash_2 );
			hash_0 -= 0.49999;
			hash_1 -= 0.49999;
			hash_2 -= 0.49999;
		
			//	evaluate gradients
			float4 grad_results = rsqrt( hash_0 * hash_0 + hash_1 * hash_1 + hash_2 * hash_2 ) * ( hash_0 * v1234_x + hash_1 * v1234_y + hash_2 * v1234_z );
		
			//	Normalization factor to scale the final result to a strict 1.0->-1.0 range
			//	x = sqrt( 0.75 ) * 0.5
			//	NF = 1.0 / ( x * ( ( 0.5 ? x*x ) ^ 3 ) * 2.0 )
			//	http://briansharpe.wordpress.com/2012/01/13/simplex-noise/#comment-36
			const float FINAL_NORMALIZATION = 37.837227241611314102871574478976;
		
			//	sum with the surflet and return
			return dot( Simplex3D_GetSurfletWeights( v1234_x, v1234_y, v1234_z ), grad_results ) * FINAL_NORMALIZATION;
		}
		float SimplexRidged(float3 p, int octaves, float3 offset, float frequency, float amplitude, float lacunarity, float persistence, float ridgeOffset)
		{
			float sum = 0;
			for (int i = 0; i < octaves; i++)
			{
				float h = 0;
				h = 0.5 * (ridgeOffset - abs(4*SimplexPerlin3D((p + offset) * frequency)));
				sum += h*amplitude;
				frequency *= lacunarity;
				amplitude *= persistence;
			}
			return sum;
		}
		
		//--- Licensing ---

		//
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


	ENDCG












	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		

		Blend SrcAlpha One

		LOD 200
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Color (0,0,0,0) }

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert decal:add noforwardadd
		#pragma target 3.0
		#pragma glsl

		float _FieldSplashDecayStartAfterLifeTimePercent;
		float _FieldSplashPow;

		float _ImpactSplashVisibility;
		float _ImpactSplashPow;

		float _FieldDischargeVisibility;
		float _FieldSplashGrowthMultiplier;
		float _FieldHighlightVisibility;
		float _FieldHighlightPow;

		float _FieldOutlineThickness;
		float _PassiveFieldVisibility;
		float _FieldRimVisibility;
		float _FieldRimColorVisibility;

		float _FieldSplashRadius;
		float _ImpactSplashRadius;
		float _ImpactSplashGrowthMultiplier;

		fixed _Octaves;
		float _Frequency;
		float _Amplitude;
		float3 _Offset;
		float _Lacunarity;
		float _Persistence;
		float _RidgeOffset;

		float _AnimSpeed;

		float _PowInner;
		float _PowOuter;

		float _PowInner2;
		float _PowOuter2;

		
		float4 _FieldColor;
		float _ScaleX;
		float _ScaleY;
		float _ScaleZ;

		uniform float3 coords[50];
		uniform float3 normals[50];
		uniform float3 times[50];
		float time;
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 normWorld;
			float3 localPos;
		};

		void vert(inout appdata_full v,out Input o){
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			o.normWorld = normalize(mul( (float3x3)unity_ObjectToWorld, v.normal));

			float3 unscaledPos = normalize(v.normal).xyz*_FieldOutlineThickness;
			v.vertex.xyz = v.vertex.xyz+float3(unscaledPos.x*(1.0/_ScaleX),unscaledPos.y*(1.0/_ScaleY),unscaledPos.z*(1.0/_ScaleZ));
			
			o.localPos = float3(v.vertex.x*(_ScaleX),v.vertex.y*(_ScaleY),v.vertex.z*(_ScaleZ));
		}

		void surf (Input IN, inout SurfaceOutput o) {

			float h = SimplexRidged(IN.localPos, _Octaves, _Offset+float3(0,_Time.x*_AnimSpeed,0), _Frequency, _Amplitude, _Lacunarity, _Persistence, _RidgeOffset);
			
			float h2 = SimplexRidged(IN.localPos, _Octaves, _Offset+float3(0,_Time.x*_AnimSpeed,0), _Frequency*3, _Amplitude*0.7, _Lacunarity, _Persistence, _RidgeOffset);
			
			float3 I = normalize(IN.worldPos - _WorldSpaceCameraPos.xyz);
			
			h = saturate(h * 0.5 + 0.5);
			
			float4 c2=0;
			float4 c3 = 0;
			
			
			float powS = _ImpactSplashPow;
			float powS2 = _FieldSplashPow;
			float hitRadius=_ImpactSplashRadius;
			float fieldRadius=_FieldSplashRadius;
			
			float decayStartMultiplier = 100.0/(100.0-_FieldSplashDecayStartAfterLifeTimePercent);
			
			for(int i=0;i<50;i++){
				
				float3 localPoint =float3(coords[i].x*(_ScaleX),coords[i].y*(_ScaleY),coords[i].z*(_ScaleZ));

				float n=15*(1.0-times[i].x);
				
				float rad = (1-times[i].x)*hitRadius*1+hitRadius*1+0.0001;
			
				rad = ((1.0-times[i].x)*_ImpactSplashGrowthMultiplier+1.0)*hitRadius+0.0001;
			
				float r = (max(0,(rad-distance(IN.localPos,localPoint))*(times[i].x)))/rad;
			
				
				float mult =((1-times[i].y)*_FieldSplashGrowthMultiplier+1)*fieldRadius+0.0001;
				float d = (max(0,(mult-distance(IN.localPos,localPoint))*min(1,times[i].y*decayStartMultiplier)))/mult;
			
				d=saturate(d);
			
				r=pow(r,powS)/(pow(r,powS)+pow(1-r,powS));
				d=pow(d,powS2)/(pow(d,powS2)+pow(1-d,powS2));
				
				float3 localNorm =mul((float3x3)unity_ObjectToWorld,normalize(normals[i]));
				float dotToImpactNormal =1.0-((dot(normalize(IN.normWorld),normalize(localNorm))+1)*0.5);
			
			
				c3.r += r*max(0,1.0-c3.r)*dotToImpactNormal;
				
				c2.r += d*max(0,1.0-c2.r)*dotToImpactNormal;
			}
			
			c3.r = c3.r*saturate(lerp(h2*c3.r,1,c3.r));
			
			
			
			half rim4 =saturate(((pow(1.0-pow(abs(dot(I, IN.normWorld)*1),_PowInner),_PowOuter))+0)/(1));
			
			rim4 = min(rim4,0.5);
			
			rim4 = rim4+(1.0-rim4)*_PassiveFieldVisibility;
			
			
			
			half rim5 =saturate(((1.0-pow(1.0-pow(abs(dot(I, IN.normWorld)*1),_PowInner2),_PowOuter2))-0.0)/(1.0));
			
			float lerpF = lerp(h*2,1,pow(rim4,4));
			
			float rimField =rim4*rim5*lerpF;
			
			
			float4 fieldEmissionColor = lerp(_FieldColor,1,saturate(pow(h*c2.r*3,3)));
			
			float c3r2p2 = pow(c3.r*2,2);
			
			float4 impactEmissionColor =lerp(_FieldColor+h*c3.r,1,c3r2p2);
			
			
			float4 emission = rimField*_FieldRimColorVisibility*_FieldRimVisibility+saturate(fieldEmissionColor)*_FieldDischargeVisibility+lerp(impactEmissionColor*1.2,c3.r*2,_FieldDischargeVisibility)*_ImpactSplashVisibility;
			
			float fieldAlpha = saturate(h*c2.r+pow(saturate(c2.r),_FieldHighlightPow)*_FieldHighlightVisibility)*1.5*_FieldDischargeVisibility;
			
			float alphaTotal =rimField*_FieldRimVisibility+fieldAlpha+pow(c3.r*2,0.9)*_ImpactSplashVisibility;
			
			
			o.Emission =  emission * alphaTotal;

			o.Alpha =1;

		}
		ENDCG
	} 
	FallBack "Transparent/Diffuse"
}
