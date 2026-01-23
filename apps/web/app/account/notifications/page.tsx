"use client";

import { useState, useEffect } from "react";
import {
  getNotifications,
  registerDevice,
  getDevices,
  deleteDevice,
  Notification,
  DeviceToken,
} from "@/lib/api";

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [devices, setDevices] = useState<DeviceToken[]>([]);
  const [loading, setLoading] = useState(true);
  const [devicesLoading, setDevicesLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedType, setSelectedType] = useState<string>("all");
  const [showDeviceModal, setShowDeviceModal] = useState(false);
  const [newDeviceToken, setNewDeviceToken] = useState("");
  const [deviceType, setDeviceType] = useState("web");

  useEffect(() => {
    fetchNotifications();
  }, [selectedType]);

  useEffect(() => {
    if (showDeviceModal) {
      fetchDevices();
    }
  }, [showDeviceModal]);

  const fetchNotifications = async () => {
    try {
      setLoading(true);
      setError(null);
      const token = localStorage.getItem("authToken");
      if (!token) {
        setError("Not authenticated");
        setLoading(false);
        return;
      }

      const result = await getNotifications(token, selectedType);
      setNotifications(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load notifications");
    } finally {
      setLoading(false);
    }
  };

  const fetchDevices = async () => {
    try {
      setDevicesLoading(true);
      const token = localStorage.getItem("authToken");
      if (!token) return;

      const result = await getDevices(token);
      setDevices(result);
    } catch (err) {
      console.error("Failed to load devices:", err);
    } finally {
      setDevicesLoading(false);
    }
  };

  const handleRegisterDevice = async () => {
    if (!newDeviceToken.trim()) {
      alert("Please enter a device token");
      return;
    }

    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        alert("Not authenticated");
        return;
      }

      await registerDevice(token, newDeviceToken.trim(), deviceType);
      setNewDeviceToken("");
      await fetchDevices();
      alert("Device token registered successfully!");
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to register device");
    }
  };

  const handleDeleteDevice = async (deviceId: string) => {
    if (!confirm("Are you sure you want to remove this device?")) {
      return;
    }

    try {
      const token = localStorage.getItem("authToken");
      if (!token) return;

      await deleteDevice(token, deviceId);
      await fetchDevices();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to delete device");
    }
  };

  const formatDate = (dateString: string): string => {
    return new Date(dateString).toLocaleString("vi-VN", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const getTypeColor = (type: string): string => {
    switch (type) {
      case "promotion":
        return "bg-green-100 text-green-800";
      case "order":
        return "bg-blue-100 text-blue-800";
      case "system":
        return "bg-gray-100 text-gray-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  if (error) {
    return (
      <div className="container mx-auto px-4 py-8" data-testid="notifications-error">
        <div className="text-center text-red-600">Error: {error}</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8" data-testid="notifications-page">
      <div className="max-w-4xl mx-auto">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">My Notifications</h1>
          <button
            onClick={() => setShowDeviceModal(!showDeviceModal)}
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
            data-testid="toggle-device-modal"
          >
            Manage Devices
          </button>
        </div>

        {/* Device Management Modal */}
        {showDeviceModal && (
          <div className="mb-6 p-4 bg-white border rounded-lg shadow" data-testid="device-modal">
            <h2 className="text-xl font-semibold mb-4">Device Token Management</h2>

            {/* Register Device Form */}
            <div className="mb-4 p-4 bg-gray-50 rounded">
              <h3 className="font-medium mb-2">Register New Device</h3>
              <div className="space-y-2">
                <input
                  type="text"
                  value={newDeviceToken}
                  onChange={(e) => setNewDeviceToken(e.target.value)}
                  placeholder="Paste FCM token here..."
                  className="w-full px-3 py-2 border rounded"
                  data-testid="device-token-input"
                />
                <select
                  value={deviceType}
                  onChange={(e) => setDeviceType(e.target.value)}
                  className="w-full px-3 py-2 border rounded"
                  data-testid="device-type-select"
                >
                  <option value="web">Web</option>
                  <option value="android">Android</option>
                  <option value="ios">iOS</option>
                </select>
                <button
                  onClick={handleRegisterDevice}
                  className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
                  data-testid="register-device-button"
                >
                  Register Device
                </button>
              </div>
            </div>

            {/* Devices List */}
            <div>
              <h3 className="font-medium mb-2">Registered Devices</h3>
              {devicesLoading ? (
                <div data-testid="devices-loading">Loading devices...</div>
              ) : devices.length === 0 ? (
                <div className="text-gray-500" data-testid="devices-empty">
                  No devices registered yet.
                </div>
              ) : (
                <div className="space-y-2" data-testid="devices-list">
                  {devices.map((device) => (
                    <div
                      key={device.id}
                      className="flex justify-between items-center p-3 bg-white border rounded"
                      data-testid={`device-item-${device.id}`}
                    >
                      <div>
                        <div className="font-mono text-sm">{device.token}</div>
                        <div className="text-xs text-gray-500">
                          {device.deviceType} • {formatDate(device.createdAt)}
                        </div>
                      </div>
                      <button
                        onClick={() => handleDeleteDevice(device.id)}
                        className="px-3 py-1 text-sm bg-red-600 text-white rounded hover:bg-red-700"
                        data-testid={`delete-device-${device.id}`}
                      >
                        Remove
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        )}

        {/* Notification Type Tabs */}
        <div className="flex gap-2 mb-6" data-testid="notification-tabs">
          {["all", "promotions", "orders", "system"].map((type) => (
            <button
              key={type}
              onClick={() => setSelectedType(type)}
              className={`px-4 py-2 rounded ${
                selectedType === type
                  ? "bg-blue-600 text-white"
                  : "bg-gray-200 text-gray-700 hover:bg-gray-300"
              }`}
              data-testid={`tab-${type}`}
            >
              {type.charAt(0).toUpperCase() + type.slice(1)}
            </button>
          ))}
        </div>

        {/* Notifications List */}
        {loading ? (
          <div className="text-center py-8" data-testid="notifications-loading">
            Loading notifications...
          </div>
        ) : notifications.length === 0 ? (
          <div className="text-center py-8 text-gray-500" data-testid="notifications-empty">
            No notifications found.
          </div>
        ) : (
          <div className="space-y-3" data-testid="notifications-list">
            {notifications.map((notification) => (
              <div
                key={notification.id}
                className={`p-4 bg-white border rounded-lg shadow-sm ${
                  notification.isRead ? "opacity-75" : ""
                }`}
                data-testid={`notification-${notification.id}`}
              >
                <div className="flex items-start justify-between mb-2">
                  <div className="flex items-center gap-2">
                    <span
                      className={`px-2 py-1 text-xs rounded ${getTypeColor(
                        notification.type
                      )}`}
                    >
                      {notification.type}
                    </span>
                    {!notification.isRead && (
                      <span className="w-2 h-2 bg-blue-600 rounded-full"></span>
                    )}
                  </div>
                  <div className="text-xs text-gray-500">
                    {formatDate(notification.createdAt)}
                  </div>
                </div>
                <h3 className="font-semibold text-lg mb-1">{notification.title}</h3>
                <p className="text-gray-700">{notification.message}</p>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
