using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {

	public ulong clientId;
	public int ColorId;

	public bool Equals(PlayerData other) {
		return clientId == other.clientId && ColorId == other.ColorId;
	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
		serializer.SerializeValue(ref clientId);
		serializer.SerializeValue(ref ColorId);
	}
}
