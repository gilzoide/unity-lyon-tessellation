use cc;

fn main() {
    cc::Build::new()
        .file("src/c/unity_helper.c")
        .compile("unity_helper");
    println!("cargo:rerun-if-changed=src/c/unity_helper.c");
}