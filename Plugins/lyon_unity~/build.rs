use cc;

fn main() {
    cc::Build::new()
        .file("src_c/unity_helper.c")
        .compile("unity_helper");
    println!("cargo:rerun-if-changed=src_c/unity_helper.c");
}