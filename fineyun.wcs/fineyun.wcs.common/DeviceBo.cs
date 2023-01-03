using Microsoft.Extensions.Configuration;

namespace fineyun.wcs.common;

public class DeviceBo
{
	public long ObjId { get; set; }
	public int ObjType { get; set; }

	public virtual RspCommon Init(IConfiguration config)
	{
		throw new NotImplementedException();
	}
	
	public virtual RspCommon InitDevices()
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