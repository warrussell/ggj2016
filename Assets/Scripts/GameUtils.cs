using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUtils 
{
	private static float GoldenRatio = (1f + Mathf.Sqrt(5f)) / 2f;
	
	public static float getFibonacciNumber(float n)
	{
		return (Mathf.Pow(GoldenRatio, n) - Mathf.Pow((1f - GoldenRatio), n)) / Mathf.Sqrt(5f);
	}
	
	public static int Now
	{
		get { return (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds; }
	}
	
	public static bool checkChance(float chance)
	{
		bool result = false;
		
		float randomNum = Random.Range(1f, 101f);
		float percentChance = chance * 100;
		
		if (randomNum <= percentChance) {
			result = true;
		}
		
		return result;
	}
	
	public static bool checkCollision(Vector2 position, string tag)
	{
		bool result = false;
		
		RaycastHit2D hitTest = Physics2D.Raycast (position, Vector2.zero);
		if(hitTest.collider != null) 
		{
			if (hitTest.collider.gameObject.tag == tag) 
			{
				result = true;
			}
		}
		
		return result;
	}
	
	public static IPoolableObject getRandomObjectFromPool(IPoolableObject[] objects)
	{
		int i;
		int sumOfWeights = 0;
		int index = -1;
		
		for (i = 0; i < objects.Length; ++i) {
			sumOfWeights += objects[i].getPoolWeight();
		}
		
		//bug with Random.Range() needs +1 to randomly choose last element in the range
		int randomNumber = Random.Range(0, sumOfWeights + 1);
		
		sumOfWeights = 0;
		for (i = 0; i < objects.Length; ++i) {
			sumOfWeights += objects[i].getPoolWeight();
			if (randomNumber <= sumOfWeights) {
				index = i;
				break;
			}
		}
		
		return objects[index];
	}
	
	public static IEnumerator RollUpTextStringNumber(Text textField, string preText, string postText, float currentValue, float targetValue, float numOfFrames)
	{
		float difference = targetValue - currentValue;
		float rate = difference / numOfFrames;
		
		while (currentValue != targetValue)
		{
			difference = Mathf.Abs(targetValue - currentValue);
			
			if (difference <= Mathf.Abs(rate)) {
				currentValue = targetValue;
			} else {
				currentValue += rate;
			}
			
			textField.text = preText + Mathf.Round(currentValue) + postText;		
			
			if (currentValue == targetValue) {
				break;
			} else {
				yield return null;
			}
		}
	}
	
	public static float GetBias (float time, float bias)
	{
		return (time / ((((1.0f / bias) - 2.0f) * (1.0f - time)) + 1.0f));
	}
	
	public static float GetGain (float time, float gain)
	{
		if (time < 0.5f)
			return GetBias (time * 2.0f, gain) / 2.0f;
		else
			return GetBias (time * 2.0f - 1.0f, 1.0f - gain) / 2.0f + 0.5f;
	}
}