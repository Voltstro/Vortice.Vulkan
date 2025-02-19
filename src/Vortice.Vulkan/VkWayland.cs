﻿// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Vulkan;

[Flags]
public enum VkWaylandSurfaceCreateFlagsKHR
{
    None = 0,
}

public unsafe struct VkWaylandSurfaceCreateInfoKHR
{
    public VkStructureType sType;
    public void* pNext;
    public VkWaylandSurfaceCreateFlagsKHR flags;
    public IntPtr display;
    public IntPtr surface;
}

public static unsafe partial class Vulkan
{
    public static readonly string VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME = "VK_KHR_wayland_surface";

    /// <summary>
    /// VK_KHR_WAYLAND_SURFACE_EXTENSION_NAME = "VK_KHR_wayland_surface"
    /// </summary>
    public static readonly string KHRWaylandSurfaceExtensionName = "VK_KHR_wayland_surface";

    private static delegate* unmanaged[Stdcall]<VkInstance, VkWaylandSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult> vkCreateWaylandSurfaceKHR_ptr;
    private static delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, VkBool32> vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr;

    private static void LoadWayland(VkInstance instance)
    {
        vkCreateWaylandSurfaceKHR_ptr = (delegate* unmanaged[Stdcall]<VkInstance, VkWaylandSurfaceCreateInfoKHR*, VkAllocationCallbacks*, VkSurfaceKHR*, VkResult>)vkGetInstanceProcAddr(instance.Handle, nameof(vkCreateWaylandSurfaceKHR));
        vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr = (delegate* unmanaged[Stdcall]<VkPhysicalDevice, uint, IntPtr, VkBool32>)vkGetInstanceProcAddr(instance.Handle, nameof(vkGetPhysicalDeviceWaylandPresentationSupportKHR));
    }

    public static unsafe VkResult vkCreateWaylandSurfaceKHR(VkInstance instance, VkWaylandSurfaceCreateInfoKHR* pCreateInfo, VkAllocationCallbacks* pAllocator, VkSurfaceKHR* pSurface)
    {
        return vkCreateWaylandSurfaceKHR_ptr(instance, pCreateInfo, pAllocator, pSurface);
    }

    public static unsafe bool vkGetPhysicalDeviceWaylandPresentationSupportKHR(VkPhysicalDevice physicalDevice, uint queueFamilyIndex, IntPtr display)
    {
        return vkGetPhysicalDeviceWaylandPresentationSupportKHR_ptr(physicalDevice, queueFamilyIndex, display);
    }
}
