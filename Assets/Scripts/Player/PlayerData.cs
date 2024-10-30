using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable {

	public ulong clientId;
	public int ColorId;
	public int points;
	public FixedString64Bytes playerName;
	public FixedString64Bytes playerId;


	public bool Equals(PlayerData other) {
		return clientId == other.clientId &&
			ColorId == other.ColorId &&
			playerName == other.playerName &&
			playerId == other.playerId &&
			points == other.points;

	}

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
		serializer.SerializeValue(ref clientId);
		serializer.SerializeValue(ref ColorId);
		serializer.SerializeValue(ref playerName);
		serializer.SerializeValue(ref playerId);
		serializer.SerializeValue(ref points);
	}
}
