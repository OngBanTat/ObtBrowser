package com.obtbrowser;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.NullNode;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.util.Map;
import java.util.stream.Collectors;

/** Low-level HTTP helper shared by all API classes. Uses Java 11 HttpClient. */
public class HttpHelper {

    /** Thrown when the API returns a 4xx or 5xx status. */
    public static class ApiException extends RuntimeException {
        public final int statusCode;
        public ApiException(int statusCode, String message) {
            super("HTTP " + statusCode + ": " + message);
            this.statusCode = statusCode;
        }
    }

    private final String baseUrl;
    private final HttpClient httpClient;
    final ObjectMapper mapper;

    public HttpHelper(String baseUrl) {
        this.baseUrl = baseUrl.replaceAll("/$", "");
        this.httpClient = HttpClient.newHttpClient();
        this.mapper = new ObjectMapper();
    }

    public JsonNode get(String path) throws Exception {
        return get(path, null);
    }

    public JsonNode get(String path, Map<String, String> queryParams) throws Exception {
        String url = buildUrl(path, queryParams);
        HttpRequest req = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .header("Accept", "application/json")
                .GET()
                .build();
        return send(req);
    }

    public JsonNode post(String path, Object body) throws Exception {
        String json = body != null ? mapper.writeValueAsString(body) : "{}";
        HttpRequest req = HttpRequest.newBuilder()
                .uri(URI.create(baseUrl + path))
                .header("Content-Type", "application/json")
                .header("Accept", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(json))
                .build();
        return send(req);
    }

    public JsonNode put(String path, Object body) throws Exception {
        String json = mapper.writeValueAsString(body);
        HttpRequest req = HttpRequest.newBuilder()
                .uri(URI.create(baseUrl + path))
                .header("Content-Type", "application/json")
                .header("Accept", "application/json")
                .PUT(HttpRequest.BodyPublishers.ofString(json))
                .build();
        return send(req);
    }

    public void delete(String path) throws Exception {
        HttpRequest req = HttpRequest.newBuilder()
                .uri(URI.create(baseUrl + path))
                .DELETE()
                .build();
        HttpResponse<String> resp = httpClient.send(req, HttpResponse.BodyHandlers.ofString());
        if (resp.statusCode() >= 400) {
            throw new ApiException(resp.statusCode(), extractError(resp.body()));
        }
    }

    private String buildUrl(String path, Map<String, String> params) {
        if (params == null || params.isEmpty()) return baseUrl + path;
        String qs = params.entrySet().stream()
                .filter(e -> e.getValue() != null)
                .map(e -> e.getKey() + "=" + e.getValue())
                .collect(Collectors.joining("&"));
        return baseUrl + path + (qs.isEmpty() ? "" : "?" + qs);
    }

    private JsonNode send(HttpRequest req) throws Exception {
        HttpResponse<String> resp = httpClient.send(req, HttpResponse.BodyHandlers.ofString());
        if (resp.statusCode() >= 400) {
            throw new ApiException(resp.statusCode(), extractError(resp.body()));
        }
        String body = resp.body();
        return (body == null || body.isBlank()) ? NullNode.getInstance() : mapper.readTree(body);
    }

    private String extractError(String body) {
        try {
            JsonNode node = mapper.readTree(body);
            if (node.has("error")) return node.get("error").asText();
        } catch (Exception ignored) {}
        return body;
    }
}
