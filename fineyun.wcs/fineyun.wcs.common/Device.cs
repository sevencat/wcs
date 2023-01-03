using Microsoft.Extensions.Configuration;

namespace fineyun.wcs.common;

public class Device
{
	public long ObjId { get; set; }
	public int ObjType { get; set; }

	//厂家
	public string Vendor { get; set; }
	public int Status { get; set; }

	public virtual RspCommon Init(IConfiguration config)
	{
		throw new NotImplementedException();
	}

	public virtual RspCommon Start()
	{
		return RspCommon.Success();
	}

	public virtual RspCommon Stop()
	{
		return RspCommon.Success();
	}

	public virtual RspCommon Fini()
	{
		throw new NotImplementedException();
	}
}