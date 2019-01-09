//By Dominic Brodeur-Gendron & Patrice Le Nouveau
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class GameEffect {

    #region SHAKE
    static bool perlinMode;
    static float perlinSpeed;

	public static void Shake(GameObject obj)
	{
		ShakeEffect (obj, 1, .2f, Vector3.zero, false);
	}

	public static void Shake(GameObject obj,float intensity)
	{
		ShakeEffect (obj, intensity, .2f, Vector3.zero, false);
		
	}

	public static void Shake(GameObject obj,float intensity,float time)
	{
		ShakeEffect (obj, intensity, time, Vector3.zero, false);
		
	}

	public static void ShakeDynamic(GameObject obj)
	{
		ShakeEffect (obj, 1, .2f, Vector3.zero, true);
	}

	public static void ShakeDynamic(GameObject obj,float intensity)
	{
		ShakeEffect (obj, intensity, .2f, Vector3.zero, true);
		
	}

	public static void ShakeDynamic(GameObject obj,float intensity,float time)
	{
		ShakeEffect (obj, intensity, time, Vector3.zero, true);
	}

	public static void ShakeRotation(GameObject obj, float intensity, Vector3 rotation, float time)
	{
		ShakeEffect (obj, intensity, time, rotation, true);
	}

    public static void ShakeModeSetPerlin(bool perlinModeOn, float speed)
    {
        perlinMode = perlinModeOn;
        perlinSpeed = speed;
    }

	static void ShakeEffect(GameObject obj, float intensity, float time, Vector3 rotation, bool isDynamic)
	{
        if (obj.GetComponent<_GEffect.ShakeClass>() == null)
        {
            obj.AddComponent<_GEffect.ShakeClass>();
            _GEffect.ShakeClass shake = obj.GetComponent<_GEffect.ShakeClass>();
            shake.Init(intensity, time, rotation, isDynamic);

            if(perlinMode)
            {
                shake.SetPerlinMode(perlinSpeed);
            }
        }
	}



    #endregion

    #region Freeze Frame
    /// <summary>
    /// Freeze frame, useful when dealing with multicamera
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="sec"></param>
    public static void FreezeFrame(GameObject gameObject, float sec)
    {
        if (gameObject.GetComponent<_GEffect.FreezeFrameClass>() == null)
            gameObject.AddComponent<_GEffect.FreezeFrameClass>().freezeSec = sec;
    }
    /// <summary>
    /// Freezes frame.
    /// </summary>
    /// <param name="sec">Sec.</param>
    public static void FreezeFrame(float sec)
	{
		if (Camera.main.gameObject.GetComponent<_GEffect.FreezeFrameClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FreezeFrameClass> ().freezeSec = sec;
	}
	
	/// <summary>
	/// Freezes the frame with default value of 0.1.
	/// </summary>
	public static void FreezeFrame()
	{
		if (Camera.main.gameObject.GetComponent<_GEffect.FreezeFrameClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FreezeFrameClass> ().freezeSec = .1f;
	}
	#endregion
	
	#region Sprite & Color
	
	/// <summary>
	/// Sins the gradient.
	/// </summary>
	/// <returns>The gradient.</returns>
	public static Color ColorLerp(Color color1, Color32 color2, float t)
	{
		return new Color 
			(
				Mathf.Lerp (color1.r, color2.r, t),
				Mathf.Lerp (color1.g, color2.g, t),
				Mathf.Lerp (color1.b, color2.b, t),
				Mathf.Lerp (color1.a, color2.a, t)
				);
	}
	public static Color SinGradient(Color color1, Color color2, float speed)
	{
		float t = (Mathf.Sin(Time.timeSinceLevelLoad * speed)+1) / 2;
		Color color = Color.Lerp(color1, color2,t);
		return color;
	}
	public static Color SinGradient(Color color1, Color color2, float time, float speed)
	{
		float t = (Mathf.Sin(time * speed)+1) / 2;
		Color color = Color.Lerp(color1, color2,t);
		return color;
	}

	public static void FlashSprite(GameObject obj, Color color,float duration)
	{
		if (obj.GetComponent<_GEffect.FlashSpriteClass> () == null)
		{
			obj.AddComponent<_GEffect.FlashSpriteClass> ();
			_GEffect.FlashSpriteClass flashSprite = obj.GetComponent<_GEffect.FlashSpriteClass> ();
			
			flashSprite.flashColor = color;
			flashSprite.duration = duration;
			flashSprite.flashSpriteEnum = _GEffect.FlashSpriteClass.FlashSpriteType.Simple;
		}
		
	}
	
	public static void FlashSprite(GameObject obj, Color color,float duration, int flashCount)
	{
		if (obj.GetComponent<_GEffect.FlashSpriteClass> () == null)
		{
			obj.AddComponent<_GEffect.FlashSpriteClass> ();
			_GEffect.FlashSpriteClass flashSprite = obj.GetComponent<_GEffect.FlashSpriteClass> ();
			
			flashSprite.flashColor = color;
			flashSprite.duration = duration;
			flashSprite.flashCount = flashCount;
			flashSprite.flashSpriteEnum = _GEffect.FlashSpriteClass.FlashSpriteType.Multiple;
		}
		
	}
	
	public static void FlashSpriteLerp(GameObject obj, Color color,float duration)
	{
		if (obj.GetComponent<_GEffect.FlashSpriteClass> () == null)
		{
			obj.AddComponent<_GEffect.FlashSpriteClass> ();
			_GEffect.FlashSpriteClass flashSprite = obj.GetComponent<_GEffect.FlashSpriteClass> ();
			flashSprite.flashColor = color;
			flashSprite.speed = duration;
			flashSprite.flashSpriteEnum = _GEffect.FlashSpriteClass.FlashSpriteType.Lerp;
		}
		
	}
	
	public static void FlashCamera(Color color, float time)
	{
		if(Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FlashCameraClass> ();
		
		Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> ().Flash (color,null, time,null);
	}
	public static void FlashCamera(Sprite image, float time)
	{
		if(Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FlashCameraClass> ();
		
		Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> ().Flash (Color.white,image, time,null);
		
	}
	public static void FlashCamera(Color color, float time,Transform canvas)
	{
		if(Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FlashCameraClass> ();
		
		Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> ().Flash (color,null, time,canvas);
	}
	public static void FlashCamera(Sprite image, float time,Transform canvas)
	{
		if(Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FlashCameraClass> ();
		
		Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> ().Flash (Color.white,image, time,canvas);
		
	}
	public static void FlashCamera(Color color,Sprite image, float time,Transform canvas)
	{
		if(Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> () == null)
			Camera.main.gameObject.AddComponent<_GEffect.FlashCameraClass> ();
		
		Camera.main.gameObject.GetComponent<_GEffect.FlashCameraClass> ().Flash (color,image, time,canvas);
		
	}
	#endregion
	
	public static void DestroyChilds(Transform parent)
	{
		if (parent.childCount != 0) 
		{
			int childs = parent.childCount;
			for (int i = 0; i <= childs - 1; i++)
				MonoBehaviour.Destroy (parent.GetChild (i).gameObject);
		}
		
	}
	public static void DestroyChilds(GameObject parent)
	{
		GameEffect.DestroyChilds (parent.transform);
	}
}
public static class GamePhysics
{
	public static Vector3 BallisticVel(Transform origin,Transform target)
	{
		Vector3 dir = target.position - origin.position;
		dir.y = 0;
		
		float dist = dir.magnitude;
		
		float vel = Mathf.Sqrt (dist * Physics.gravity.magnitude);
		
		return vel * dir.normalized;
	}
	
	public static Vector3 BallisticVel(Transform origin,Transform target, float angle)
	{
		Vector3 dir = target.position - origin.position;
		float h = dir.y;
		dir.y = 0;
		
		float dist = dir.magnitude;
		float a = angle * Mathf.Deg2Rad;
		
		dir.y = dist * Mathf.Tan (a);
		dist += h / Mathf.Tan (a);
		
		float vel = Mathf.Sqrt (dist * Physics.gravity.magnitude / Mathf.Sin(2.5f * a));
		return vel * dir.normalized ;
	}
}
public static class GameMath
{
	///// <summary>
	/// Calculate the horizontal position of a given index if you have n objects.
	/// Everything will be align by center
	/// Ideally, you call this function in a for loop to position every objects
	/// 
	/// for(i ...)
	/// 	objects[i].position = new Vector2(CenterAlign(objects.length, dist, i), yPos);
	/// </summary>
	/// <returns>The horizontal position</returns>
	/// <param name="NumberOfObject">Number of objects.</param>
	/// <param name="distance">Distance between each objects.</param>
	/// <param name="i">The index of the object.</param>
	public static float CenterAlign(int NumberOfObject, float distance, int i)
	{
		return ((i - (((NumberOfObject) - 1) / 2)) * distance) - (((NumberOfObject + 1) % 2) * (distance / 2));
	}

	public static float PerlinNoiseNegOneToOne(float x, float y)
	{
		return (Mathf.PerlinNoise(x,y) * 2) - 1;
	}

    public static Vector3 SphericalRotation(float radius, float theta, float phi)
    {
        float x = radius * Mathf.Sin(theta) * Mathf.Cos(phi);
        float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
        float z = radius * Mathf.Cos(theta);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// A tester
    /// </summary>
    /// <param name="i"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static Vector2 GetXYFromIndex(int i, int width)
    {
        float x = (float)i % width;
        float y = (float)i / width;
        return new Vector2(x, y);
    }

    public static int GetIndexFromXY(int x, int y, int width)
    {
        return y * width + x;
    }
    #region Curves

    public static float Stretch(float A, float stretchAmount)
	{
		if (A > 1)
			A = 1;
		
		float C = Mathf.Abs (A - .5f);
		
		return .5f +  stretchAmount + ((C + .5f) * stretchAmount);
	}

	/// <summary>
	/// Return a log function between 0 and 1
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	public static float Log01(float t)
	{
		return Mathf.Log10 ((t + .11f) * 9f);
	}
	/// <summary>
	/// Return a smooth Sigmoid between 0 and 1
	/// </summary>
	/// <param name="x">The t coordinate between 0 and 1.</param>
	public static float Sigmoid01(float t)
	{
		return Mathf.Clamp01 (1 / (1 + Mathf.Exp (-(10 * t - 5.1f))));
	}
	/// <summary>
	/// Return a steep sigmoid between 0 and 1
	/// </summary>
	/// <param name="t">The t coordinate between 0 and 1.</param>
	public static float SteepSigmoid01(float t)
	{
		return Mathf.Clamp01 (1 / (1 + Mathf.Exp (-(30 * t - 15))));
	}
	/// <summary>
	/// Return a zigzag from 0 to 1
	/// </summary>
	/// <returns>The zag01.</returns>
	/// <param name="t">T.</param>
	public static float ZigZag01(float t)
	{
		return Mathf.Clamp01((.9f * Mathf.Sin (t * 1.37f) + .1f * Mathf.Sin (t * 1.37f * 15)) * 1.02f);
	}
	public static float ExtremeExp01(float t)
	{
		return Mathf.Clamp01 (5.9f * Mathf.Exp(t * 10 - 11.77f));
	}
	/// <summary>
	/// Bounce between 0 and 1
	/// </summary>
	public static float Bounce01(float t) //from Mathfx
	{
		return Mathf.Abs (Mathf.Sin (6.28f * (t + 1) * (t + 1)) * (1 - t));
	}
	#endregion

	#region Distance
	public static bool IsMinimumDistance(GameObject object1,GameObject object2,float minimumDistance)
	{
		if (Mathf.Abs(object1.transform.position.x - object2.transform.position.x) < minimumDistance &&
		    Mathf.Abs(object1.transform.position.y - object2.transform.position.y) < minimumDistance &&
		    Mathf.Abs(object1.transform.position.z - object2.transform.position.z) < minimumDistance)
			return true;
		
		return false;
	}
	public static bool IsMinimumDistance(Transform object1,Transform object2,float minimumDistance)
	{
		if (Mathf.Abs(object1.position.x - object2.position.x) < minimumDistance &&
		    Mathf.Abs(object1.position.y - object2.position.y) < minimumDistance &&
		    Mathf.Abs(object1.position.z - object2.position.z) < minimumDistance)
			return true;
		
		return false;
	}
	public static bool IsMinimumDistance(Vector3 object1,Vector3 object2,float minimumDistance)
	{
		if (Mathf.Abs(object1.x - object2.x) < minimumDistance &&
		    Mathf.Abs(object1.y - object2.y) < minimumDistance &&
		    Mathf.Abs(object1.z - object2.z) < minimumDistance)
			return true;
		
		return false;
	}
	public static bool IsMinimumDistance(Vector2 object1,Vector2 object2,float minimumDistance)
	{
		if (Mathf.Abs(object1.x - object2.x) < minimumDistance &&
		    Mathf.Abs(object1.y - object2.y) < minimumDistance)
			return true;
		
		return false;
	}

	public static float DistanceXY(GameObject object1,GameObject object2)
	{
		return Mathf.Sqrt
			( 
			 Mathf.Pow((object1.transform.position.x - object2.transform.position.x), 2) +
			 Mathf.Pow((object1.transform.position.y - object2.transform.position.y), 2)
			 );
	}
	public static float DistanceXY(Transform transform1,Transform transform2)
	{
		return Mathf.Sqrt
			( 
			 Mathf.Pow((transform1.position.x - transform2.position.x), 2) +
			 Mathf.Pow((transform1.position.y - transform2.position.y), 2)
			 );
	}
	public static float DistanceXZ(GameObject object1,GameObject object2)
	{
		return Mathf.Sqrt
			( 
			 Mathf.Pow((object1.transform.position.x - object2.transform.position.x), 2) +
			 Mathf.Pow((object1.transform.position.z - object2.transform.position.z), 2)
			 );
	}
	public static float DistanceXZ(Transform transform1,Transform transform2)
	{
		return Mathf.Sqrt
			( 
			 Mathf.Pow((transform1.position.x - transform2.position.x), 2) +
			 Mathf.Pow((transform1.position.z - transform2.position.z), 2)
			 );
	}
	public static float DistanceYZ(GameObject object1,GameObject object2)
	{
		return Mathf.Sqrt
			( 
			 Mathf.Pow((object1.transform.position.y - object2.transform.position.y), 2) +
			 Mathf.Pow((object1.transform.position.z - object2.transform.position.z), 2)
			 );
	}
	public static float DistanceYZ(Transform transform1,Transform transform2)
	{
		return Mathf.Sqrt
			( 
			 Mathf.Pow((transform1.position.y - transform2.position.y), 2) +
			 Mathf.Pow((transform1.position.z - transform2.position.z), 2)
			 );
	}
    #endregion

    #region Vectors

    public static Vector3 GetNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Vector3.Cross(p2 - p1, p3 - p1).normalized;
    }

    public static Vector2 PolarToCartesian(float radius, float theta)
    {
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        return new Vector2(x, y);
    }


    /// <summary>
    /// </summary>
    /// <param name="radiusAndTheta">A Vector2 containing x = radius, y = theta</param>
    /// <returns></returns>
    public static Vector2 PolarToCartesian(Vector2 radiusAndTheta)
    {
        return PolarToCartesian(radiusAndTheta.x, radiusAndTheta.y);
    }

    /// <summary>
    /// Start with a x,y and receive a Vector2(Radius, Theta)
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector2 CartesianToPolar(Vector2 position)
    {
        float r = Mathf.Sqrt(position.x * position.x + position.y * position.y);
        float theta = Mathf.Atan2(position.y, position.x);
        return new Vector2(r, theta);
    }

    public static Vector2 CartesianToPolar(float x, float y)
    {
        return CartesianToPolar(new Vector2(x,y));
    }

    public static Vector2 RotateVector(float angle, Vector2 point)
    {
        float a = angle * Mathf.PI / 180;
        float cosA = Mathf.Cos(a);
        float sinA = Mathf.Sin(a);
        Vector2 newPoint =
            new Vector2(
                (point.x * cosA - point.y * sinA),
                (point.x * sinA + point.y * cosA)
                );
        return newPoint;
    }

    public static Vector3 RotateVectorX(float angle, Vector3 point)
    {
        Vector2 vec = RotateVector(angle, new Vector2(point.z, point.y));
        return new Vector3(point.x, vec.y, vec.x);
    }

    public static Vector3 RotateVectorZ(float angle, Vector3 point)
    {
        Vector2 vec = RotateVector(angle, new Vector2(point.x, point.y));
        return new Vector3(vec.y, vec.x, point.z);
    }

    public static Vector3 RotateVectorY(float angle, Vector3 point)
	{
		Vector2 vec = RotateVector(angle,  new Vector2 (point.x, point.z));
		return new Vector3 (vec.x, point.y, vec.y);
	}

    public static Vector2 RandomRotateVector(Vector2 vector, float min, float max)
    {
        return RotateVector(Random.Range(min, max), vector);
    }

    public static Vector2 RandomRotateVectorX(Vector3 vector, float min, float max)
    {
        return RotateVectorX(Random.Range(min, max), vector);
    }
    public static Vector2 RandomRotateVectorY(Vector3 vector, float min, float max)
    {
        return RotateVectorY(Random.Range(min, max), vector);
    }
    public static Vector2 RandomRotateVectorZ(Vector3 vector, float min, float max)
    {
        return RotateVectorZ(Random.Range(min, max), vector);
    }
    public static Vector2 PolarRose(float theta, float k, float radius)
    {
        float r = radius * Mathf.Cos(k * theta);
        float x = r * Mathf.Cos(theta);
        float y = r * Mathf.Sin(theta);
        return new Vector2(x, y);
    }
    public static Vector2[] GetPolarRoseCoordinate(float k, float radius, int resolution)
    {
        return GetPolarRoseCoordinate(k, radius, resolution, 1);
    }
    public static Vector2[] GetPolarRoseCoordinate(float k, float radius, int resolution, float numberLoop)
    {
        Vector2[] points = new Vector2[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t =  (float)i / (resolution-1);
            float theta = Mathf.Lerp(0, Mathf.PI * 2 * numberLoop, t);
            points[i] = PolarRose(theta, k, radius);
        }
        return points;
    }

    public static Vector3[] GetPolarRoseCoordinateVector3(float k, float radius, int resolution)
    {
        return GetPolarRoseCoordinateVector3(k, radius, resolution, 1);
    }

    public static Vector3[] GetPolarRoseCoordinateVector3(float k, float radius, int resolution, float numberLoop)
    {
        Vector2[] points = GetPolarRoseCoordinate(k, radius, resolution, numberLoop);
        Vector3[] points3 = new Vector3[resolution];
        for (int i = 0; i < points3.Length; i++)
        {
            points3[i] = points[i];
        }
        return points3;
    }
    /// <summary>
    /// Return the angle of two vectors from -180 to 180 degree
    /// </summary>
    public static float Angle(Vector3 from, Vector3 to)
	{
		return (Vector3.Angle (from,to))* (-Mathf.Sign (Vector3.Cross (from, to).y));
	}
	
	/// <summary>
	/// Return the angle of two vectors from -180 to 180 degree (to test lol)
	/// </summary>
	public static float Angle(Vector2 from, Vector2 to)
	{
		///to test
		return (Vector2.Angle (from,to))* (-Mathf.Sign (Vector3.Cross (from, to).y));
	}
	#endregion
}
#region hidden functions
namespace _GEffect
{
	public class FreezeFrameClass : MonoBehaviour {
		
		public float freezeSec;
		
		void Start()
		{
			StartCoroutine (FreezeFrameEffect());
		}
		
		IEnumerator FreezeFrameEffect()
		{
			Time.timeScale = 0f;
			float pauseEndTime = Time.realtimeSinceStartup + freezeSec;
			while (Time.realtimeSinceStartup < pauseEndTime)
				yield return 0;
			
			Time.timeScale = 1;
			Destroy (this);
		}
	}
	
	public class ShakeClass : MonoBehaviour
	{
		float intensity;
		float currentTime;
        float time;
		Vector3 rotation;
		bool isDynamic;
		bool isPerlinMode = false;
		Vector3 originalPos;
		Vector3 originalRotation;

        Vector3 perlinSeeds;
        float perlinSpeed;

        public void SetPerlinMode(float speed)
        {
            isPerlinMode = true;
            perlinSpeed = speed;
        }

        public void Init(float intensity, float time, Vector3 rotation, bool isDynamic)
		{
			this.intensity = intensity;
			this.currentTime = time;
            this.time = time;
			this.rotation = rotation;
			this.isDynamic = isDynamic;

            perlinSeeds = Random.insideUnitSphere * 50;
        }
		
		void OnEnable()
		{
			originalPos = transform.localPosition;
			originalRotation = transform.localEulerAngles;
		}
		
		void Update()
		{
			if (isDynamic)
			{	
				originalPos = transform.localPosition;
			}
		}

		void LateUpdate()
		{
			if (currentTime > 0)
			{
                float t = currentTime / time;
                float currentIntensity = Mathf.Lerp(0, intensity, GameMath.Sigmoid01(t));

                if (isPerlinMode)
				{
					transform.localEulerAngles = originalRotation + RandomRotationPerlin();
					transform.localPosition = originalPos + RandomPerlinPosition() * currentIntensity;
				}
				else
				{
					transform.localEulerAngles = originalRotation + RandomRotation();
					transform.localPosition = originalPos + Random.insideUnitSphere * currentIntensity;
				}

                currentTime -= Time.deltaTime;
			}
			else
			{
                currentTime = 0f;
				transform.localPosition = originalPos;
				transform.localEulerAngles = originalRotation;

                Destroy(this);
			}
		}

		Vector3 RandomPerlinPosition()
		{
            perlinSeeds += Vector3.one * Time.deltaTime * perlinSpeed;

            return new Vector3(
                GameMath.PerlinNoiseNegOneToOne(perlinSeeds.x, perlinSeeds.y),
                GameMath.PerlinNoiseNegOneToOne(perlinSeeds.x, perlinSeeds.z),
                GameMath.PerlinNoiseNegOneToOne(perlinSeeds.y, perlinSeeds.z)
				);
		}

		Vector3 RandomRotation()
		{
			if(rotation == Vector3.zero)
			{	
				return Vector3.zero;
			}
			else
			{
				return new Vector3(
					rotation.x * Random.Range(-1.0f,1.0f),
					rotation.y * Random.Range(-1.0f,1.0f),
					rotation.z * Random.Range(-1.0f,1.0f)
					);
			}
		}
		
		Vector3 RandomRotationPerlin()
		{
			if(rotation == Vector3.zero)
			{	
				return Vector3.zero;
			}
			else
			{
                return RandomPerlinPosition();

            }
		}
	}
	
	public class FlashSpriteClass : MonoBehaviour
	{
		public enum FlashSpriteType
		{
			Multiple, Simple, Lerp
		};
		
		public FlashSpriteType flashSpriteEnum;
		
		Color originalColor;
		public Color flashColor;
		
		public float speed, duration;
		float t;
		public int flashCount;
		
		SpriteRenderer spriteRender;
		
		void Start()
		{
			spriteRender = gameObject.GetComponent<SpriteRenderer> ();
			originalColor = spriteRender.color;
			
			if (flashSpriteEnum == FlashSpriteType.Simple) 
			{
				StartCoroutine(simpleFlash ());
			}
			else if (flashSpriteEnum == FlashSpriteType.Multiple)
			{
				StartCoroutine(multipleFlash ());
			}
			
		}
		
		void Update()
		{
			if (flashSpriteEnum == FlashSpriteType.Lerp)
			{
				lerpFlash ();
			}
		}
		
		IEnumerator simpleFlash()
		{
			
			spriteRender.color = flashColor;
			yield return new WaitForSeconds (duration);
			spriteRender.color = originalColor;
			Destroy (this);
		}
		
		IEnumerator multipleFlash()
		{
			float splitTime = (duration / flashCount) / 2;
			
			for(int i = 0; i < flashCount; i++)
			{
				spriteRender.color = flashColor;
				yield return new WaitForSeconds (splitTime);
				spriteRender.color = originalColor;
				yield return new WaitForSeconds (splitTime);
			}
			Destroy (this);
		}
		
		void lerpFlash()
		{
			t += Time.deltaTime / speed;
			spriteRender.color = new Color
				(
					Mathf.Lerp(originalColor.r, flashColor.r,t),
					Mathf.Lerp(originalColor.g, flashColor.g,t),
					Mathf.Lerp(originalColor.b, flashColor.b,t),
					Mathf.Lerp(originalColor.a, flashColor.a,t)		
					);
			if (t > 1) 
			{
				spriteRender.color = originalColor;
				Destroy (this);
			}
		}
		
	}
	public class FlashCameraClass : MonoBehaviour
	{
		float t = 0;
		float speed;
		GameObject screen;
		Color color;
		Image image;
		
		bool isFlashing = false;
		bool isIncreasing = true;
		Transform canvas;

		void Awake()
		{
			screen = new GameObject ();
			screen.AddComponent<Image> ();
			screen.GetComponent<Image> ().raycastTarget = false;
			screen.name = "Flashing Screen";
			screen.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
		}

		void SetCanvas()
		{
			if(canvas == null)
				screen.transform.SetParent (GameObject.Find ("Canvas").transform, true);
			else
				screen.transform.SetParent (canvas, true);
			
			screen.GetComponent<Image> ().rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);
			screen.GetComponent<Image> ().rectTransform.localPosition = Vector2.zero;
		}
		
		public void Flash(Color _color, Sprite sprite, float time, Transform _canvas)
		{
			if (_canvas != null)
			{
				if(canvas != _canvas)
					canvas = _canvas;		
				
				SetCanvas ();
			}
			
			speed = 1 / time;
			t = 0;
			color = _color;
			screen.GetComponent<Image> ().sprite = sprite;
			isIncreasing = true;
			isFlashing = true;
		}
		void Update()
		{
			if (!isFlashing)
				return;
			
			t += Time.deltaTime * speed * 2;
			
			
			if (isIncreasing)
			{
				screen.GetComponent<Image> ().color  = new Color (color.r, color.g, color.b, Mathf.Lerp (0,  color.a, t));
				if (t > 1)
				{
					isIncreasing = false;
					t = 0;
				}
			}
			else
			{
				screen.GetComponent<Image> ().color  = new Color (color.r, color.g, color.b, Mathf.Lerp (color.a, 0, t));
				if (t > 1)
					isFlashing = false;
			}
		}
	}
}
#endregion
