using System.ComponentModel.DataAnnotations;


namespace Entity;


/// <summary>
/// <para>Entity view model for listing purposes.</para>
/// <para>Static members are thread safe, instance members are not.</para>
/// </summary>
public class EntityForL
{
	/// <summary>
	/// Entity ID.
	/// </summary>
	public int Id {get; set;}

	/// <summary>
	/// Date.
	/// </summary>
	public DateTime Date {get; set;}

	/// <summary>
	/// Name.
	/// </summary>
	public string Name {get; set;}

	/// <summary>
	/// Condition. In range [0;10].
	/// </summary>
	public int Condition {get; set;}

	/// <summary>
	/// Indicates if entity is deletable.
	/// </summary>
	public bool Deletable {get; set;}
}

/// <summary>
/// <para>Entity view model for create/update purposes.</para>
/// <para>Static members are thread safe, instance members are not.</para>
/// </summary>
public class EntityForCU
{
	/// <summary>
	/// Entity ID. Ignored when creating.
	/// </summary>
	public int Id {get; set;}

	/// <summary>
	/// Date. Required.
	/// </summary>
	[Required]
	public DateTime Date {get; set;}

	/// <summary>
	/// Name. Required.
	/// </summary>
	[Required]
	public string Name {get; set;}

	/// <summary>
	/// Condition. In range [0;10].
	/// </summary>
	[Range(0, 10)]
	public int Condition {get; set;}

	/// <summary>
	/// Indicates if entity is deletable.
	/// </summary>
	public bool Deletable {get; set;}

}