import React from "react";

function Navbar() {
  return (
    <nav className="bg-blue-900 text-white shadow-md">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between items-center h-16">
          {/* Logo / Brand */}
          <div className="flex-shrink-0 text-xl font-bold">
            OurVLE
          </div>

          {/* Links */}
          <div className="hidden md:flex space-x-6">
            <a href="/" className="hover:text-blue-300 transition">
              Home
            </a>
            <a href="/courses" className="hover:text-blue-300 transition">
              Courses
            </a>
            <a href="/discussions" className="hover:text-blue-300 transition">
              Discussions
            </a>
            <a href="/profile" className="hover:text-blue-300 transition">
              Profile
            </a>
          </div>

          {/* Mobile Menu Button */}
          <div className="md:hidden">
            <button className="focus:outline-none">
              <svg
                className="h-6 w-6"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" 
                  d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
