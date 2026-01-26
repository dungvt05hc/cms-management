"use client";

import { useState, useRef, useCallback } from "react";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5027";

interface ImageUploadProps {
  images: string[];
  onImagesChange: (images: string[]) => void;
  token: string;
  maxImages?: number;
}

interface PreviewImage {
  file: File;
  preview: string;
  uploading: boolean;
  error?: string;
}

export default function ImageUpload({
  images,
  onImagesChange,
  token,
  maxImages = 10,
}: ImageUploadProps) {
  const [previews, setPreviews] = useState<PreviewImage[]>([]);
  const [isDragging, setIsDragging] = useState(false);
  const [urlInput, setUrlInput] = useState("");
  const fileInputRef = useRef<HTMLInputElement>(null);

  const allowedTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];
  const maxFileSize = 5 * 1024 * 1024; // 5MB

  const validateFile = (file: File): string | null => {
    if (!allowedTypes.includes(file.type)) {
      return `Invalid file type: ${file.name}. Allowed: JPG, PNG, GIF, WebP`;
    }
    if (file.size > maxFileSize) {
      return `File too large: ${file.name}. Max size: 5MB`;
    }
    return null;
  };

  const uploadFile = async (file: File): Promise<string | null> => {
    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await fetch(`${API_BASE_URL}/admin/uploads/image`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || "Upload failed");
      }

      const data = await response.json();
      return data.url;
    } catch (error) {
      console.error("Upload error:", error);
      throw error;
    }
  };

  const handleFiles = useCallback(
    async (files: FileList | File[]) => {
      const fileArray = Array.from(files);
      const remainingSlots = maxImages - images.length - previews.length;

      if (fileArray.length > remainingSlots) {
        alert(`You can only add ${remainingSlots} more image(s)`);
        return;
      }

      // Create previews
      const newPreviews: PreviewImage[] = [];
      for (const file of fileArray) {
        const error = validateFile(file);
        if (error) {
          alert(error);
          continue;
        }

        const preview = URL.createObjectURL(file);
        newPreviews.push({ file, preview, uploading: true });
      }

      setPreviews((prev) => [...prev, ...newPreviews]);

      // Upload files
      for (let i = 0; i < newPreviews.length; i++) {
        const previewItem = newPreviews[i];
        try {
          const url = await uploadFile(previewItem.file);
          if (url) {
            onImagesChange([...images, url]);
            // Remove from previews after successful upload
            setPreviews((prev) =>
              prev.filter((p) => p.preview !== previewItem.preview)
            );
            URL.revokeObjectURL(previewItem.preview);
          }
        } catch (error) {
          setPreviews((prev) =>
            prev.map((p) =>
              p.preview === previewItem.preview
                ? { ...p, uploading: false, error: "Upload failed" }
                : p
            )
          );
        }
      }
    },
    [images, previews.length, maxImages, token, onImagesChange]
  );

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(true);
  }, []);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  }, []);

  const handleDrop = useCallback(
    (e: React.DragEvent) => {
      e.preventDefault();
      e.stopPropagation();
      setIsDragging(false);

      const files = e.dataTransfer.files;
      if (files.length > 0) {
        handleFiles(files);
      }
    },
    [handleFiles]
  );

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files && files.length > 0) {
      handleFiles(files);
    }
    // Reset input
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleUrlAdd = () => {
    if (urlInput.trim()) {
      if (images.length >= maxImages) {
        alert(`Maximum ${maxImages} images allowed`);
        return;
      }
      onImagesChange([...images, urlInput.trim()]);
      setUrlInput("");
    }
  };

  const removeImage = (index: number) => {
    onImagesChange(images.filter((_, i) => i !== index));
  };

  const removePreview = (preview: string) => {
    setPreviews((prev) => prev.filter((p) => p.preview !== preview));
    URL.revokeObjectURL(preview);
  };

  const moveImage = (fromIndex: number, toIndex: number) => {
    if (toIndex < 0 || toIndex >= images.length) return;
    const newImages = [...images];
    const [moved] = newImages.splice(fromIndex, 1);
    newImages.splice(toIndex, 0, moved);
    onImagesChange(newImages);
  };

  return (
    <div>
      {/* Drag and Drop Zone */}
      <div
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={() => fileInputRef.current?.click()}
        style={{
          border: `2px dashed ${isDragging ? "#007bff" : "#ccc"}`,
          borderRadius: 8,
          padding: 40,
          textAlign: "center",
          backgroundColor: isDragging ? "#f0f7ff" : "#fafafa",
          cursor: "pointer",
          transition: "all 0.2s ease",
          marginBottom: 16,
        }}
      >
        <input
          ref={fileInputRef}
          type="file"
          accept="image/jpeg,image/png,image/gif,image/webp"
          multiple
          onChange={handleFileSelect}
          style={{ display: "none" }}
        />
        <div style={{ fontSize: 48, marginBottom: 12 }}>📸</div>
        <p style={{ margin: 0, color: "#666", fontSize: 16 }}>
          <strong>Drag & drop images here</strong>
        </p>
        <p style={{ margin: "8px 0 0", color: "#999", fontSize: 14 }}>
          or click to select files (JPG, PNG, GIF, WebP • Max 5MB each)
        </p>
      </div>

      {/* URL Input */}
      <div style={{ display: "flex", gap: 12, marginBottom: 16 }}>
        <input
          type="url"
          value={urlInput}
          onChange={(e) => setUrlInput(e.target.value)}
          placeholder="Or enter image URL directly"
          onKeyDown={(e) => e.key === "Enter" && (e.preventDefault(), handleUrlAdd())}
          style={{
            flex: 1,
            padding: 10,
            border: "1px solid #ddd",
            borderRadius: 4,
          }}
        />
        <button
          type="button"
          onClick={handleUrlAdd}
          style={{
            padding: "10px 20px",
            backgroundColor: "#28a745",
            color: "white",
            border: "none",
            borderRadius: 4,
            cursor: "pointer",
          }}
        >
          Add URL
        </button>
      </div>

      {/* Image Previews (uploading) */}
      {previews.length > 0 && (
        <div style={{ marginBottom: 16 }}>
          <h4 style={{ margin: "0 0 12px", color: "#666" }}>Uploading...</h4>
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fill, minmax(150px, 1fr))",
              gap: 16,
            }}
          >
            {previews.map((preview) => (
              <div
                key={preview.preview}
                style={{
                  position: "relative",
                  border: preview.error ? "2px solid #dc3545" : "1px solid #ddd",
                  borderRadius: 4,
                  overflow: "hidden",
                  opacity: preview.uploading ? 0.7 : 1,
                }}
              >
                <img
                  src={preview.preview}
                  alt="Preview"
                  style={{ width: "100%", height: 120, objectFit: "cover" }}
                />
                {preview.uploading && !preview.error && (
                  <div
                    style={{
                      position: "absolute",
                      top: 0,
                      left: 0,
                      right: 0,
                      bottom: 0,
                      backgroundColor: "rgba(255,255,255,0.8)",
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                    }}
                  >
                    <div style={{ fontSize: 14, color: "#007bff" }}>Uploading...</div>
                  </div>
                )}
                {preview.error && (
                  <div
                    style={{
                      position: "absolute",
                      bottom: 0,
                      left: 0,
                      right: 0,
                      backgroundColor: "#dc3545",
                      color: "white",
                      padding: 4,
                      fontSize: 12,
                      textAlign: "center",
                    }}
                  >
                    {preview.error}
                  </div>
                )}
                <button
                  type="button"
                  onClick={() => removePreview(preview.preview)}
                  style={{
                    position: "absolute",
                    top: 4,
                    right: 4,
                    backgroundColor: "#dc3545",
                    color: "white",
                    border: "none",
                    borderRadius: "50%",
                    width: 24,
                    height: 24,
                    cursor: "pointer",
                    fontSize: 16,
                    lineHeight: "22px",
                  }}
                >
                  ×
                </button>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Uploaded Images */}
      {images.length > 0 ? (
        <div>
          <h4 style={{ margin: "0 0 12px", color: "#666" }}>
            Product Images ({images.length}/{maxImages})
          </h4>
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(auto-fill, minmax(150px, 1fr))",
              gap: 16,
            }}
          >
            {images.map((url, index) => (
              <div
                key={`${url}-${index}`}
                style={{
                  position: "relative",
                  border: index === 0 ? "2px solid #007bff" : "1px solid #ddd",
                  borderRadius: 4,
                  overflow: "hidden",
                }}
              >
                {index === 0 && (
                  <div
                    style={{
                      position: "absolute",
                      top: 0,
                      left: 0,
                      backgroundColor: "#007bff",
                      color: "white",
                      padding: "2px 8px",
                      fontSize: 10,
                      fontWeight: "bold",
                    }}
                  >
                    MAIN
                  </div>
                )}
                <img
                  src={url}
                  alt={`Product ${index + 1}`}
                  style={{ width: "100%", height: 120, objectFit: "cover" }}
                  onError={(e) => {
                    (e.target as HTMLImageElement).src =
                      "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='150' height='120'%3E%3Crect fill='%23f0f0f0' width='100%25' height='100%25'/%3E%3Ctext fill='%23999' x='50%25' y='50%25' text-anchor='middle' dy='.3em'%3EImage Error%3C/text%3E%3C/svg%3E";
                  }}
                />
                {/* Reorder buttons */}
                <div
                  style={{
                    position: "absolute",
                    bottom: 4,
                    left: 4,
                    display: "flex",
                    gap: 4,
                  }}
                >
                  {index > 0 && (
                    <button
                      type="button"
                      onClick={() => moveImage(index, index - 1)}
                      style={{
                        backgroundColor: "rgba(0,0,0,0.6)",
                        color: "white",
                        border: "none",
                        borderRadius: 4,
                        width: 24,
                        height: 24,
                        cursor: "pointer",
                        fontSize: 12,
                      }}
                      title="Move left"
                    >
                      ←
                    </button>
                  )}
                  {index < images.length - 1 && (
                    <button
                      type="button"
                      onClick={() => moveImage(index, index + 1)}
                      style={{
                        backgroundColor: "rgba(0,0,0,0.6)",
                        color: "white",
                        border: "none",
                        borderRadius: 4,
                        width: 24,
                        height: 24,
                        cursor: "pointer",
                        fontSize: 12,
                      }}
                      title="Move right"
                    >
                      →
                    </button>
                  )}
                </div>
                {/* Delete button */}
                <button
                  type="button"
                  onClick={() => removeImage(index)}
                  style={{
                    position: "absolute",
                    top: 4,
                    right: 4,
                    backgroundColor: "#dc3545",
                    color: "white",
                    border: "none",
                    borderRadius: "50%",
                    width: 24,
                    height: 24,
                    cursor: "pointer",
                    fontSize: 16,
                    lineHeight: "22px",
                  }}
                >
                  ×
                </button>
              </div>
            ))}
          </div>
        </div>
      ) : (
        <p style={{ color: "#666", fontStyle: "italic" }}>No images added yet</p>
      )}
    </div>
  );
}
